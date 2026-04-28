using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Interface.Workflows;
using InventoryTrackingAutomation.Events.Workflows;
using InventoryTrackingAutomation.Models.Movements;
using InventoryTrackingAutomation.Workflows;
using Volo.Abp.EventBus.Local;

namespace InventoryTrackingAutomation.Managers.Movements;

// Hareket talebi domain manager'i - MovementRequest icin is kurallari, FK validasyonu ve workflow tetikleme.
//işlevi: Hareket taleplerinin (MovementRequest) oluşturulması, güncellenmesi ve workflow süreçlerinin başlatılmasını yönetir.
//sistemdeki görevii: Talep aşamasındaki iş kurallarını (validasyonlar, benzersizlik kontrolleri) uygular ve onay sürecini tetikler.
public class MovementRequestManager : BaseManager<MovementRequest>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IWorkerRepository _workerRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly Managers.Workflows.WorkflowManager _workflowManager;
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMovementRequestLineRepository _movementRequestLineRepository;
    private readonly ILocalEventBus _localEventBus;
    private readonly IMapper _mapper;

    public MovementRequestManager(
        IMovementRequestRepository repository,
        IWarehouseRepository warehouseRepository,
        IWorkerRepository workerRepository,
        IVehicleRepository vehicleRepository,
        Managers.Workflows.WorkflowManager workflowManager,
        IWorkflowDefinitionRepository workflowDefinitionRepository,
        IWorkflowInstanceRepository workflowInstanceRepository,
        IProductRepository productRepository,
        IMovementRequestLineRepository movementRequestLineRepository,
        ILocalEventBus localEventBus,
        IMapper mapper)
        : base(repository)
    {
        _mapper = mapper;
        _warehouseRepository = warehouseRepository;
        _workerRepository = workerRepository;
        _vehicleRepository = vehicleRepository;
        _workflowManager = workflowManager;
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _workflowInstanceRepository = workflowInstanceRepository;
        _productRepository = productRepository;
        _movementRequestLineRepository = movementRequestLineRepository;
        _localEventBus = localEventBus;
    }

    // Yeni hareket talebi entity'si olusturur; persist sorumlulugu cagirandadir.
    public async Task<MovementRequest> CreateAsync(CreateMovementRequestModel model)
    {
        await ValidateRequestNumberForCreateAsync(model.RequestNumber);
        await ValidateHeaderReferencesAsync(model.RequestedByWorkerId, model.SourceWarehouseId, model.TargetWarehouseId, model.RequestedVehicleId);
        await ValidatePriorityAsync(model.Priority);

        var entity = new MovementRequest(GuidGenerator.Create());
        _mapper.Map(model, entity);
        entity.Status = InventoryTrackingAutomation.Enums.MovementStatusEnum.Pending;
        return entity;
    }

    // Hareket talebini gunceller; degisen alanlar icin validasyon yapar.
    public async Task<MovementRequest> UpdateAsync(MovementRequest existing, UpdateMovementRequestModel model)
    {
        await ValidateRequestNumberForUpdateAsync(existing, model.RequestNumber);
        await ValidateChangedReferencesAsync(existing, model);
        await ValidatePriorityAsync(model.Priority);
        await ValidateStatusAsync(model.Status);

        _mapper.Map(model, existing);
        return existing;
    }

    // Hareket talebini olusturur, workflow'u baslatir ve veritabanina kaydeder.
    public async Task<MovementRequest> CreateWithWorkflowAsync(CreateMovementRequestModel model, Guid currentUserId)
    {
        var entity = await CreateAsync(model);
        var workflowInstance = await AssignWorkflowAsync(entity, currentUserId);

        var inserted = await Repository.InsertAsync(entity, autoSave: true);
        await PublishInitialWorkflowStepAssignedAsync(workflowInstance);
        return inserted;
    }

    // Birden fazla hareket talebini workflow ile birlikte sirasiyla olusturur.
    public async Task<List<MovementRequest>> CreateManyWithWorkflowAsync(List<CreateMovementRequestModel> models, Guid currentUserId)
    {
        var entities = new List<MovementRequest>();
        var workflowInstances = new List<InventoryTrackingAutomation.Entities.Workflows.WorkflowInstance>();

        foreach (var model in models)
        {
            var entity = await CreateAsync(model);
            var workflowInstance = await AssignWorkflowAsync(entity, currentUserId);
            if (workflowInstance != null)
            {
                workflowInstances.Add(workflowInstance);
            }

            entities.Add(entity);
        }

        var inserted = await Repository.InsertManyAndGetListAsync(entities);
        foreach (var workflowInstance in workflowInstances)
        {
            await PublishInitialWorkflowStepAssignedAsync(workflowInstance);
        }

        return inserted;
    }

    // Talep header'i, satirlari ve workflow'u ayni UnitOfWork icinde olusturur.
    public async Task<MovementRequest> CreateWithLinesAndWorkflowAsync(
        CreateMovementRequestWithLinesModel model,
        Guid currentUserId)
    {
        await ValidateRequestNumberForCreateAsync(model.RequestNumber);
        await ValidateHeaderReferencesAsync(model.RequestedByWorkerId, model.SourceWarehouseId, model.TargetWarehouseId, model.RequestedVehicleId);
        await ValidatePriorityAsync(model.Priority);
        await ValidateLineProductsAsync(model);

        var entity = new MovementRequest(GuidGenerator.Create());
        _mapper.Map(model, entity);
        entity.Status = InventoryTrackingAutomation.Enums.MovementStatusEnum.Pending;

        var workflowInstance = await AssignWorkflowAsync(entity, currentUserId);
        var insertedHeader = await Repository.InsertAsync(entity, autoSave: true);

        var lineEntities = model.Lines.Select(lineModel =>
        {
            var lineEntity = new MovementRequestLine(GuidGenerator.Create());
            _mapper.Map(lineModel, lineEntity);
            lineEntity.MovementRequestId = insertedHeader.Id;
            return lineEntity;
        }).ToList();

        await _movementRequestLineRepository.InsertManyAsync(lineEntities, autoSave: true);
        await PublishInitialWorkflowStepAssignedAsync(workflowInstance);
        return insertedHeader;
    }

    private async Task ValidateRequestNumberForCreateAsync(string requestNumber)
    {
        // Talep numarasi doluysa ayni numara ile ikinci talep acilamaz.
        if (!string.IsNullOrWhiteSpace(requestNumber))
        {
            await EnsureUniqueAsync(x => x.RequestNumber == requestNumber);
        }
    }

    private async Task ValidateRequestNumberForUpdateAsync(MovementRequest existing, string requestNumber)
    {
        // Talep numarasi degisiyorsa mevcut kayit haric unique kontrolu yapilir.
        if (!string.IsNullOrWhiteSpace(requestNumber) && existing.RequestNumber != requestNumber)
        {
            await EnsureUniqueAsync(x => x.RequestNumber == requestNumber, existing.Id);
        }
    }

    private async Task ValidateHeaderReferencesAsync(
        Guid requestedByWorkerId,
        Guid SourceWarehouseId,
        Guid TargetWarehouseId,
        Guid requestedVehicleId)
    {
        // Header FK'lari tek noktada dogrulanir; AppService is kurali tasimaz.
        await EnsureExistsInAsync(_workerRepository, requestedByWorkerId);
        await EnsureExistsInAsync(_warehouseRepository, SourceWarehouseId);
        await EnsureExistsInAsync(_warehouseRepository, TargetWarehouseId);
        await ValidateRequestedVehicleAvailableAsync(requestedVehicleId);
    }

    private async Task ValidateChangedReferencesAsync(MovementRequest existing, UpdateMovementRequestModel model)
    {
        // Update sirasinda sadece degisen FK alanlari yeniden dogrulanir.
        if (existing.RequestedByWorkerId != model.RequestedByWorkerId)
        {
            await EnsureExistsInAsync(_workerRepository, model.RequestedByWorkerId);
        }

        if (existing.SourceWarehouseId != model.SourceWarehouseId)
        {
            await EnsureExistsInAsync(_warehouseRepository, model.SourceWarehouseId);
        }

        if (existing.TargetWarehouseId != model.TargetWarehouseId)
        {
            await EnsureExistsInAsync(_warehouseRepository, model.TargetWarehouseId);
        }

        if (existing.RequestedVehicleId != model.RequestedVehicleId)
        {
            await ValidateRequestedVehicleAvailableAsync(model.RequestedVehicleId);
        }
    }

    private async Task ValidatePriorityAsync(InventoryTrackingAutomation.Enums.MovementPriorityEnum priority)
    {
        // Priority allowed listesi settings uzerinden gelir; enum degeri hardcode edilmez.
        await EnsureValidEnumAsync(
            priority,
            Settings.InventoryTrackingAutomationSettings.Movements.AllowedMovementPriorities);
    }

    private async Task ValidateStatusAsync(InventoryTrackingAutomation.Enums.MovementStatusEnum status)
    {
        // Status allowed listesi settings uzerinden gelir; enum degeri hardcode edilmez.
        await EnsureValidEnumAsync(
            status,
            Settings.InventoryTrackingAutomationSettings.Movements.AllowedMovementStatuses);
    }

    private async Task ValidateLineProductsAsync(CreateMovementRequestWithLinesModel model)
    {
        // Satirlardaki product id'ler tek sorgu ile dogrulanir.
        var productIds = model.Lines.Select(l => l.ProductId).Distinct().ToList();
        await EnsureAllExistInAsync(_productRepository, productIds);
    }

    private async Task ValidateRequestedVehicleAvailableAsync(Guid requestedVehicleId)
    {
        // Secilen aracin var ve aktif olmasi zorunludur; aktif gorev kontrolu VehicleTask fazinda yapilacak.
        var vehicle = await _vehicleRepository.FindAsync(requestedVehicleId);
        if (vehicle == null)
        {
            throw new Volo.Abp.BusinessException(InventoryTrackingAutomationErrorCodes.Vehicles.NotFound);
        }

        if (!vehicle.IsActive)
        {
            throw new Volo.Abp.BusinessException(InventoryTrackingAutomationErrorCodes.General.InvalidOperation);
        }
    }

    private async Task<InventoryTrackingAutomation.Entities.Workflows.WorkflowInstance?> AssignWorkflowAsync(
        MovementRequest entity,
        Guid currentUserId)
    {
        // MovementRequest icin aktif workflow definition yoksa geriye donuk uyumluluk icin workflow baslatilmaz.
        var workflowDef = await _workflowDefinitionRepository.FindAsync(
            w => w.Name == WorkflowDefinitionNames.MovementRequest && w.IsActive);
        if (workflowDef == null)
        {
            return null;
        }

        var startModel = new InventoryTrackingAutomation.Models.Workflows.StartWorkflowModel
        {
            WorkflowDefinitionId = workflowDef.Id,
            EntityType = WorkflowEntityTypes.MovementRequest,
            EntityId = entity.Id,
            InitiatorUserId = currentUserId
        };

        var workflowInstance = await _workflowManager.StartWorkflowAsync(startModel);
        await _workflowInstanceRepository.InsertAsync(workflowInstance);
        entity.WorkflowInstanceId = workflowInstance.Id;
        entity.Status = InventoryTrackingAutomation.Enums.MovementStatusEnum.InReview;
        return workflowInstance;
    }

    private Task PublishInitialWorkflowStepAssignedAsync(InventoryTrackingAutomation.Entities.Workflows.WorkflowInstance? workflowInstance)
    {
        var firstStep = workflowInstance?.Steps.FirstOrDefault();
        if (workflowInstance == null || firstStep == null)
        {
            return Task.CompletedTask;
        }

        return _localEventBus.PublishAsync(new WorkflowStepAssignedEto
        {
            WorkflowInstanceId = workflowInstance.Id,
            WorkflowInstanceStepId = firstStep.Id,
            WorkflowStepDefinitionId = firstStep.WorkflowStepDefinitionId,
            EntityType = workflowInstance.EntityType,
            EntityId = workflowInstance.EntityId,
            AssignedUserId = firstStep.AssignedUserId
        });
    }
}
