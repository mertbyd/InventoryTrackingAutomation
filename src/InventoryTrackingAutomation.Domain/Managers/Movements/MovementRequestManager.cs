using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Interface.Shipments;
using InventoryTrackingAutomation.Interface.Workflows;
using InventoryTrackingAutomation.Models.Movements;
using InventoryTrackingAutomation.Workflows;

namespace InventoryTrackingAutomation.Managers.Movements;

// Hareket talebi domain manager'ı — MovementRequest entity'si için iş kuralları, FK ve enum validasyonu, workflow tetikleme.
public class MovementRequestManager : BaseManager<MovementRequest>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IWorkerRepository _workerRepository;
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly Managers.Workflows.WorkflowManager _workflowManager;
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMovementRequestLineRepository _movementRequestLineRepository;

    private readonly IMapper _mapper;
    public MovementRequestManager(
        IMovementRequestRepository repository,
        ISiteRepository siteRepository,
        IWorkerRepository workerRepository,
        IShipmentRepository shipmentRepository,
        IVehicleRepository vehicleRepository,
        Managers.Workflows.WorkflowManager workflowManager,
        IWorkflowDefinitionRepository workflowDefinitionRepository,
        IWorkflowInstanceRepository workflowInstanceRepository,
        IProductRepository productRepository,
        IMovementRequestLineRepository movementRequestLineRepository,
        IMapper mapper)
        : base(repository)
    {
        _mapper = mapper;
        _siteRepository = siteRepository;
        _workerRepository = workerRepository;
        _shipmentRepository = shipmentRepository;
        _vehicleRepository = vehicleRepository;
        _workflowManager = workflowManager;
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _workflowInstanceRepository = workflowInstanceRepository;
        _productRepository = productRepository;
        _movementRequestLineRepository = movementRequestLineRepository;
    }

    // Yeni hareket talebi entity'si oluşturur — RequestNumber unique, FK ve enum validasyonu yapar, Status'u Pending olarak ayarlar.
    // Persist sorumluluğu çağırana aittir; bu metot DB'ye yazmaz.
    public async Task<MovementRequest> CreateAsync(CreateMovementRequestModel model)
    {
        // RequestNumber verilmişse benzersiz olduğunu doğrula.
        if (!string.IsNullOrWhiteSpace(model.RequestNumber))
        {
            await EnsureUniqueAsync(
                x => x.RequestNumber == model.RequestNumber);
        }

        // Talebi yapan worker'ın varlığını doğrula.
        await EnsureExistsInAsync(
            _workerRepository,
            model.RequestedByWorkerId);

        // Kaynak ve hedef lokasyonların varlığını doğrula.
        await EnsureExistsInAsync(
            _siteRepository,
            model.SourceSiteId);
        await EnsureExistsInAsync(
            _siteRepository,
            model.TargetSiteId);

        // İlişkili sevkiyat varsa varlığını doğrula.
        await EnsureExistsInAsync(
            _shipmentRepository,
            model.ShipmentId);

        await ValidateRequestedVehicleAvailableAsync(model.RequestedVehicleId);

        // Priority değerinin allowed listesinde olduğunu doğrula.
        // Status doğrulanmaz: yeni talep her zaman sunucu tarafında Pending olarak başlar.
        await EnsureValidEnumAsync(model.Priority, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Movements.AllowedMovementPriorities);

        // Model → Entity map ve yeni Id ata.
        var entity = new MovementRequest(GuidGenerator.Create());
        _mapper.Map(model, entity);

        // Yeni talep her zaman Pending durumunda başlar (input'tan gelen Status göz ardı edilir).
        entity.Status = InventoryTrackingAutomation.Enums.MovementStatusEnum.Pending;
        return entity;
    }

    // Hareket talebini günceller — değişen alanlar için unique/FK/enum kontrolleri yapar, mevcut entity'yi map'ler.
    public async Task<MovementRequest> UpdateAsync(MovementRequest existing, UpdateMovementRequestModel model)
    {
        // RequestNumber değişiyorsa kendisi hariç benzersizliği doğrula.
        if (!string.IsNullOrWhiteSpace(model.RequestNumber) && existing.RequestNumber != model.RequestNumber)
        {
            await EnsureUniqueAsync(
                x => x.RequestNumber == model.RequestNumber,
                existing.Id);
        }

        // Talep eden worker değişiyorsa varlığını doğrula.
        if (existing.RequestedByWorkerId != model.RequestedByWorkerId)
        {
            await EnsureExistsInAsync(
                _workerRepository,
                model.RequestedByWorkerId);
        }

        // Kaynak lokasyon değişiyorsa varlığını doğrula.
        if (existing.SourceSiteId != model.SourceSiteId)
        {
            await EnsureExistsInAsync(
                _siteRepository,
                model.SourceSiteId);
        }

        // Hedef lokasyon değişiyorsa varlığını doğrula.
        if (existing.TargetSiteId != model.TargetSiteId)
        {
            await EnsureExistsInAsync(
                _siteRepository,
                model.TargetSiteId);
        }

        // Sevkiyat değişiyorsa varlığını doğrula.
        if (model.ShipmentId.HasValue && existing.ShipmentId != model.ShipmentId)
        {
            await EnsureExistsInAsync(
                _shipmentRepository,
                model.ShipmentId.Value);
        }

        if (existing.RequestedVehicleId != model.RequestedVehicleId)
        {
            await ValidateRequestedVehicleAvailableAsync(model.RequestedVehicleId);
        }

        // Priority ve Status değerlerinin allowed listesinde olduğunu doğrula.
        await EnsureValidEnumAsync(model.Priority, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Movements.AllowedMovementPriorities);
        await EnsureValidEnumAsync(model.Status, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Movements.AllowedMovementStatuses);

        // Update model'ini mevcut entity üzerine map'le.
        _mapper.Map(model, existing);
        return existing;
    }

    // Hareket talebini oluşturur, iş akışını başlatır ve veritabanına kaydeder — service'in doğrudan çağırdığı tek metot.
    public async Task<MovementRequest> CreateWithWorkflowAsync(CreateMovementRequestModel model, Guid currentUserId)
    {
        // Önce entity'yi iş kurallarıyla oluştur.
        var entity = await CreateAsync(model);

        // Sonra iş akışını ata (workflow definition aktifse).
        await AssignWorkflowAsync(entity, currentUserId);

        // Persist et ve generated Id ile entity'yi döndür.
        return await Repository.InsertAsync(entity, autoSave: true);
    }

    // Birden fazla hareket talebini sırayla oluşturur, her biri için iş akışı başlatır ve toplu kaydeder.
    public async Task<List<MovementRequest>> CreateManyWithWorkflowAsync(List<CreateMovementRequestModel> models, Guid currentUserId)
    {
        var entities = new List<MovementRequest>();

        // Her model için Create + Workflow assignment uygula.
        foreach (var model in models)
        {
            var entity = await CreateAsync(model);
            await AssignWorkflowAsync(entity, currentUserId);
            entities.Add(entity);
        }

        // Toplu InsertManyAndGetListAsync ile veritabanına yaz.
        return await Repository.InsertManyAndGetListAsync(entities);
    }

    // Talebi + satırlarını + workflow'u tek transaction'da oluşturur.
    // AppService [UnitOfWork] içinden çağırır; tüm insert'ler aynı UoW'da commit edilir.
    public async Task<MovementRequest> CreateWithLinesAndWorkflowAsync(
        CreateMovementRequestWithLinesModel model,
        Guid currentUserId)
    {
        // 1. Header validasyonu — mevcut CreateAsync'in benzeri.
        if (!string.IsNullOrWhiteSpace(model.RequestNumber))
        {
            await EnsureUniqueAsync(x => x.RequestNumber == model.RequestNumber);
        }

        await EnsureExistsInAsync(_workerRepository, model.RequestedByWorkerId);
        await EnsureExistsInAsync(_siteRepository, model.SourceSiteId);
        await EnsureExistsInAsync(_siteRepository, model.TargetSiteId);
        await ValidateRequestedVehicleAvailableAsync(model.RequestedVehicleId);
        await EnsureValidEnumAsync(
            model.Priority,
            InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Movements.AllowedMovementPriorities);

        // 2. Lines'taki product'ların hepsinin var olduğunu toplu doğrula.
        var productIds = model.Lines.Select(l => l.ProductId).Distinct().ToList();
        await EnsureAllExistInAsync(_productRepository, productIds);

        // 3. Header entity'sini oluştur.
        var entity = new MovementRequest(GuidGenerator.Create());
        _mapper.Map(model, entity);
        entity.Status = InventoryTrackingAutomation.Enums.MovementStatusEnum.Pending;

        // 4. Workflow ata (definition aktifse Status InReview'e geçer).
        await AssignWorkflowAsync(entity, currentUserId);

        // 5. Header'ı insert et — line'ların FK'sı için Id lazım.
        var insertedHeader = await Repository.InsertAsync(entity, autoSave: true);

        // 6. Lines entity'lerini oluştur ve insert et.
        var lineEntities = model.Lines.Select(lineModel =>
        {
            var lineEntity = new MovementRequestLine(GuidGenerator.Create());
            _mapper.Map(lineModel, lineEntity);
            lineEntity.MovementRequestId = insertedHeader.Id;
            return lineEntity;
        }).ToList();

        await _movementRequestLineRepository.InsertManyAsync(lineEntities, autoSave: true);

        return insertedHeader;
    }

    // Talepte secilen arac aktif ve baska aktif sevkiyatta degilse kullanilabilir.
    private async Task ValidateRequestedVehicleAvailableAsync(Guid requestedVehicleId)
    {
        var vehicle = await _vehicleRepository.FindAsync(requestedVehicleId);
        if (vehicle == null)
        {
            throw new Volo.Abp.BusinessException(InventoryTrackingAutomationErrorCodes.Vehicles.NotFound)
                .WithData("VehicleId", requestedVehicleId);
        }

        if (!vehicle.IsActive)
        {
            throw new Volo.Abp.BusinessException(InventoryTrackingAutomationErrorCodes.General.InvalidOperation)
                .WithData("Message", "Seçilen araç aktif değil.")
                .WithData("VehicleId", requestedVehicleId);
        }

        var activeShipments = await _shipmentRepository.GetListAsync(x =>
            x.VehicleId == requestedVehicleId &&
            (x.Status == InventoryTrackingAutomation.Enums.ShipmentStatusEnum.Preparing ||
             x.Status == InventoryTrackingAutomation.Enums.ShipmentStatusEnum.InTransit));
        var activeShipment = activeShipments.FirstOrDefault();

        if (activeShipment != null)
        {
            throw new Volo.Abp.BusinessException(InventoryTrackingAutomationErrorCodes.General.InvalidOperation)
                .WithData("Message", "Seçilen araç aktif bir sevkiyatta olduğu için atanamaz.")
                .WithData("VehicleId", requestedVehicleId)
                .WithData("ShipmentId", activeShipment.Id);
        }
    }

    // Aktif MovementRequest workflow definition'ını bulur, initiator bağlamıyla iş akışını başlatır
    // ve oluşan WorkflowInstanceId'yi entity'ye yazar.
    // Initiator'ın yöneticisini bulma sorumluluğu WorkflowManager → IApproverStrategy zincirine aittir.
    private async Task AssignWorkflowAsync(MovementRequest entity, Guid currentUserId)
    {
        // MovementRequest için aktif workflow definition'ı bul; yoksa workflow başlatılmaz (geriye dönük uyumluluk).
        var workflowDef = await _workflowDefinitionRepository.FindAsync(
            w => w.Name == WorkflowDefinitionNames.MovementRequest && w.IsActive);

        if (workflowDef == null)
        {
            return;
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

        // Workflow başarıyla atandı — talep artık inceleme sürecinde.
        entity.Status = InventoryTrackingAutomation.Enums.MovementStatusEnum.InReview;
    }
}
