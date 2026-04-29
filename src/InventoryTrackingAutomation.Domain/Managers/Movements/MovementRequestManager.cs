using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Interface.Workflows;
using InventoryTrackingAutomation.Events.Workflows;
using InventoryTrackingAutomation.Models.Movements;
using InventoryTrackingAutomation.Managers.Inventory;
using InventoryTrackingAutomation.Managers.Tasks;
using InventoryTrackingAutomation.Models.Inventory;
using InventoryTrackingAutomation.Workflows;
using Volo.Abp.EventBus.Local;
using InventoryTrackingAutomation.Interface.Tasks;
using Volo.Abp;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Managers.Movements;

// Hareket talebi domain manager'i - MovementRequest icin is kurallari, FK validasyonu ve workflow tetikleme.
//işlevi: Hareket taleplerinin (MovementRequest) oluşturulması, güncellenmesi ve workflow süreçlerinin başlatılmasını yönetir.
//sistemdeki görevii: Talep aşamasındaki iş kurallarını (validasyonlar, benzersizlik kontrolleri) uygular ve onay sürecini tetikler.
//işlevi: MovementRequest etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class MovementRequestManager : BaseManager<MovementRequest>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IWorkerRepository _workerRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IInventoryTaskRepository _inventoryTaskRepository;
    private readonly Managers.Workflows.WorkflowManager _workflowManager;
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMovementRequestLineRepository _movementRequestLineRepository;
    private readonly StockTransferManager _stockTransferManager;
    private readonly StockAdjustmentManager _stockAdjustmentManager;
    private readonly VehicleTaskManager _vehicleTaskManager;
    private readonly ILocalEventBus _localEventBus;
    private readonly IMapper _mapper;

    public MovementRequestManager(
        IMovementRequestRepository repository,
        IWarehouseRepository warehouseRepository,
        IWorkerRepository workerRepository,
        IVehicleRepository vehicleRepository,
        IInventoryTaskRepository inventoryTaskRepository,
        Managers.Workflows.WorkflowManager workflowManager,
        IWorkflowDefinitionRepository workflowDefinitionRepository,
        IWorkflowInstanceRepository workflowInstanceRepository,
        IProductRepository productRepository,
        IMovementRequestLineRepository movementRequestLineRepository,
        StockTransferManager stockTransferManager,
        StockAdjustmentManager stockAdjustmentManager,
        VehicleTaskManager vehicleTaskManager,
        ILocalEventBus localEventBus,
        IMapper mapper)
        : base(repository)
    {
        _mapper = mapper;
        _warehouseRepository = warehouseRepository;
        _workerRepository = workerRepository;
        _vehicleRepository = vehicleRepository;
        _inventoryTaskRepository = inventoryTaskRepository;
        _workflowManager = workflowManager;
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _workflowInstanceRepository = workflowInstanceRepository;
        _productRepository = productRepository;
        _movementRequestLineRepository = movementRequestLineRepository;
        _stockTransferManager = stockTransferManager;
        _stockAdjustmentManager = stockAdjustmentManager;
        _vehicleTaskManager = vehicleTaskManager;
        _localEventBus = localEventBus;
    }

    /// Yeni hareket talebi entity'si oluşturmak için kullanılır.
    public async Task<MovementRequest> CreateAsync(CreateMovementRequestModel model)
    {
        await ValidateRequestNumberForCreateAsync(model.RequestNumber);
        await ValidateHeaderReferencesAsync(model.RequestedByWorkerId, model.SourceWarehouseId, model.TargetWarehouseId, model.RequestedVehicleId, model.AssignedTaskId);
        await ValidatePriorityAsync(model.Priority);

        var entity = new MovementRequest(GuidGenerator.Create());
        _mapper.Map(model, entity);
        entity.Type = ResolveRequestType(entity.AssignedTaskId);
        entity.Status = InventoryTrackingAutomation.Enums.MovementStatusEnum.Pending;
        return entity;
    }

    /// Mevcut bir hareket talebini güncellemek için kullanılır.
    public async Task<MovementRequest> UpdateAsync(MovementRequest existing, UpdateMovementRequestModel model)
    {
        EnsureEditable(existing);
        await ValidateRequestNumberForUpdateAsync(existing, model.RequestNumber);
        await ValidateChangedReferencesAsync(existing, model);
        await ValidatePriorityAsync(model.Priority);
        await ValidateStatusAsync(model.Status);

        _mapper.Map(model, existing);
        return existing;
    }

    /// Hareket talebini workflow ile birlikte oluşturmak için kullanılır.
    public async Task<MovementRequest> CreateWithWorkflowAsync(CreateMovementRequestModel model, Guid currentUserId)
    {
        var entity = await CreateAsync(model);
        var workflowInstance = await AssignWorkflowAsync(entity, currentUserId);

        var inserted = await Repository.InsertAsync(entity, autoSave: true);
        if (workflowInstance != null)
        {
            await PublishInitialWorkflowStepAssignedAsync(workflowInstance);
        }
        return inserted;
    }

    /// Birden fazla hareket talebini workflow ile birlikte oluşturmak için kullanılır.
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

    /// Hareket talebini satırları ve workflow'u ile birlikte oluşturmak için kullanılır.
    public async Task<MovementRequest> CreateWithLinesAndWorkflowAsync(
        CreateMovementRequestWithLinesModel model,
        Guid currentUserId)
    {
        await ValidateRequestNumberForCreateAsync(model.RequestNumber);
        await ValidateHeaderReferencesAsync(model.RequestedByWorkerId, model.SourceWarehouseId, model.TargetWarehouseId, model.RequestedVehicleId, model.AssignedTaskId);
        await ValidatePriorityAsync(model.Priority);
        await ValidateLineProductsAsync(model);

        var entity = new MovementRequest(GuidGenerator.Create());
        _mapper.Map(model, entity);
        entity.Type = ResolveRequestType(entity.AssignedTaskId);
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
        if (workflowInstance != null)
        {
            await PublishInitialWorkflowStepAssignedAsync(workflowInstance);
        }
        return insertedHeader;
    }

    /// Hareket talebinin sevkiyat işlemini başlatmak için kullanılır.
    [UnitOfWork]
    public async Task<MovementRequest> DispatchAsync(
        Guid requestId,
        string? dispatchNote,
        Guid currentUserId,
        Guid currentWorkerId)
    {
        var request = await EnsureExistsAsync(requestId);
        if (request.Type == MovementRequestTypeEnum.TaskReturnToWarehouse)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequests.DispatchNotAllowed)
                .WithData("MovementRequestId", request.Id)
                .WithData("Type", request.Type);
        }

        EnsureStatus(request, MovementStatusEnum.Approved, InventoryTrackingAutomationErrorCodes.MovementRequests.DispatchNotAllowed);

        if (MissingId(request.RequestedVehicleId))
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequests.VehicleRequired)
                .WithData("MovementRequestId", request.Id);
        }

        await ValidateRequestedVehicleAvailableAsync(request.RequestedVehicleId.Value);
        await ValidateDispatchTaskStatusAsync(request);

        var lines = await GetLinesForTransferAsync(request.Id);
        foreach (var line in lines)
        {
            await _stockTransferManager.ExecuteAsync(new StockTransferModel
            {
                ProductId = line.ProductId,
                Quantity = line.Quantity,
                SourceLocationType = StockLocationTypeEnum.Warehouse,
                SourceLocationId = request.SourceWarehouseId,
                DestinationLocationType = StockLocationTypeEnum.Vehicle,
                DestinationLocationId = request.RequestedVehicleId.Value,
                TransactionType = InventoryTransactionTypeEnum.WarehouseToVehicle,
                RelatedMovementRequestId = request.Id,
                RelatedTaskId = request.AssignedTaskId,
                PerformedByUserId = currentUserId,
                Note = dispatchNote
            });
        }

        if (request.AssignedTaskId.HasValue)
        {
            await _vehicleTaskManager.EnsureAssignedAsync(
                request.AssignedTaskId.Value,
                request.RequestedVehicleId.Value,
                currentWorkerId);
        }

        request.Status = MovementStatusEnum.Shipped;
        return await Repository.UpdateAsync(request, autoSave: true);
    }

    /// Hareket talebinin varış noktasında teslim alınması için kullanılır.
    [UnitOfWork]
    public async Task<MovementRequest> ReceiveAsync(
        Guid requestId,
        ReceiveMovementRequestModel model,
        Guid currentUserId)
    {
        var request = await EnsureExistsAsync(requestId);
        EnsureStatus(request, MovementStatusEnum.Shipped, InventoryTrackingAutomationErrorCodes.MovementRequests.ReceiveNotAllowed);

        if (request.Type == MovementRequestTypeEnum.TaskReturnToWarehouse)
        {
            return await ReceiveTaskReturnAsync(request, model, currentUserId);
        }

        if (MissingId(request.RequestedVehicleId))
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequests.VehicleRequired)
                .WithData("MovementRequestId", request.Id);
        }

        if (request.AssignedTaskId.HasValue)
        {
            request.Status = MovementStatusEnum.Completed;
            return await Repository.UpdateAsync(request, autoSave: true);
        }

        if (MissingId(request.TargetWarehouseId))
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequests.TargetRequired)
                .WithData("MovementRequestId", request.Id);
        }

        var lines = await GetLinesForTransferAsync(request.Id);
        foreach (var line in lines)
        {
            await _stockTransferManager.ExecuteAsync(new StockTransferModel
            {
                ProductId = line.ProductId,
                Quantity = line.Quantity,
                SourceLocationType = StockLocationTypeEnum.Vehicle,
                SourceLocationId = request.RequestedVehicleId.Value,
                DestinationLocationType = StockLocationTypeEnum.Warehouse,
                DestinationLocationId = request.TargetWarehouseId.Value,
                TransactionType = InventoryTransactionTypeEnum.VehicleToWarehouse,
                RelatedMovementRequestId = request.Id,
                RelatedTaskId = null,
                PerformedByUserId = currentUserId,
                Note = model.ReceiveNote
            });
        }

        request.Status = MovementStatusEnum.Completed;
        return await Repository.UpdateAsync(request, autoSave: true);
    }

    /// Görev iadesi sürecinde malzemelerin teslim alınması için kullanılır.
    private async Task<MovementRequest> ReceiveTaskReturnAsync(
        MovementRequest request,
        ReceiveMovementRequestModel model,
        Guid currentUserId)
    {
        if (MissingId(request.RequestedVehicleId))
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequests.VehicleRequired)
                .WithData("MovementRequestId", request.Id);
        }

        if (MissingId(request.TargetWarehouseId))
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequests.TargetRequired)
                .WithData("MovementRequestId", request.Id);
        }

        if (MissingId(request.AssignedTaskId))
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.InventoryTasks.NotFound)
                .WithData("MovementRequestId", request.Id);
        }

        var lines = await GetLinesForTransferAsync(request.Id);
        ValidateReturnReceiveInput(request, lines, model);

        foreach (var line in lines)
        {
            var receivedLine = model.Lines.Single(x => x.ProductId == line.ProductId);
            if (receivedLine.ReceivedQuantity > 0)
            {
                await _stockTransferManager.ExecuteAsync(new StockTransferModel
                {
                    ProductId = line.ProductId,
                    Quantity = receivedLine.ReceivedQuantity,
                    SourceLocationType = StockLocationTypeEnum.Vehicle,
                    SourceLocationId = request.RequestedVehicleId.Value,
                    DestinationLocationType = StockLocationTypeEnum.Warehouse,
                    DestinationLocationId = request.TargetWarehouseId.Value,
                    TransactionType = InventoryTransactionTypeEnum.VehicleToWarehouse,
                    RelatedMovementRequestId = request.Id,
                    RelatedTaskId = request.AssignedTaskId,
                    PerformedByUserId = currentUserId,
                    Note = receivedLine.Note ?? model.ReceiveNote
                });
            }

            var adjustmentQuantity = receivedLine.DamagedQuantity + receivedLine.LostQuantity + receivedLine.ConsumedQuantity;
            if (adjustmentQuantity > 0)
            {
                await _stockAdjustmentManager.DecreaseAsync(new StockAdjustmentModel
                {
                    ProductId = line.ProductId,
                    Quantity = adjustmentQuantity,
                    SourceLocationType = StockLocationTypeEnum.Vehicle,
                    SourceLocationId = request.RequestedVehicleId.Value,
                    RelatedMovementRequestId = request.Id,
                    RelatedTaskId = request.AssignedTaskId,
                    PerformedByUserId = currentUserId,
                    Note = BuildReturnAdjustmentNote(receivedLine, model.ReceiveNote)
                });
            }

            line.ReceivedQuantity = receivedLine.ReceivedQuantity;
            line.DamagedQuantity = receivedLine.DamagedQuantity;
            line.LostQuantity = receivedLine.LostQuantity;
            line.ConsumedQuantity = receivedLine.ConsumedQuantity;
            line.ReceiveNote = receivedLine.Note;
            await _movementRequestLineRepository.UpdateAsync(line, autoSave: true);
        }

        request.Status = MovementStatusEnum.Completed;
        var saved = await Repository.UpdateAsync(request, autoSave: true);
        await _vehicleTaskManager.ReleaseForTaskVehicleAsync(request.AssignedTaskId.Value, request.RequestedVehicleId.Value);
        return saved;
    }

    /// Kayıt oluştururken talep numarasının benzersizliğini doğrulamak için kullanılır.
    private async Task ValidateRequestNumberForCreateAsync(string requestNumber)
    {
        if (!string.IsNullOrWhiteSpace(requestNumber))
        {
            await EnsureUniqueAsync(x => x.RequestNumber == requestNumber);
        }
    }

    /// Talebin düzenlenebilir durumda olup olmadığını kontrol etmek için kullanılır.
    private static void EnsureEditable(MovementRequest request)
    {
        if (request.Status is MovementStatusEnum.Pending or MovementStatusEnum.InReview)
        {
            return;
        }

        throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequests.InvalidStateTransition)
            .WithData("MovementRequestId", request.Id)
            .WithData("CurrentStatus", request.Status)
            .WithData("AllowedStatuses", $"{MovementStatusEnum.Pending},{MovementStatusEnum.InReview}");
    }

    /// Talebin belirli bir statüde olup olmadığını doğrulamak için kullanılır.
    private static void EnsureStatus(MovementRequest request, MovementStatusEnum expectedStatus, string errorCode)
    {
        if (request.Status == expectedStatus)
        {
            return;
        }

        throw new BusinessException(errorCode)
            .WithData("MovementRequestId", request.Id)
            .WithData("CurrentStatus", request.Status)
            .WithData("ExpectedStatus", expectedStatus);
    }

    /// Görev atamasına göre talep türünü belirlemek için kullanılır.
    private static MovementRequestTypeEnum ResolveRequestType(Guid? assignedTaskId)
    {
        return HasValidId(assignedTaskId)
            ? MovementRequestTypeEnum.WarehouseToTask
            : MovementRequestTypeEnum.WarehouseToWarehouse;
    }

    /// İade alım giriş verilerini doğrulamak için kullanılır.
    private static void ValidateReturnReceiveInput(
        MovementRequest request,
        IReadOnlyCollection<MovementRequestLine> expectedLines,
        ReceiveMovementRequestModel model)
    {
        if (model.Lines.Count == 0)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequests.ReturnReceiveLineRequired)
                .WithData("MovementRequestId", request.Id);
        }

        var duplicateProductIds = model.Lines
            .GroupBy(x => x.ProductId)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToList();

        if (duplicateProductIds.Count > 0)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequests.QuantityMismatch)
                .WithData("MovementRequestId", request.Id)
                .WithData("DuplicateProductIds", string.Join(",", duplicateProductIds));
        }

        var expectedProductIds = expectedLines.Select(x => x.ProductId).ToHashSet();
        var unexpectedProductIds = model.Lines
            .Where(x => !expectedProductIds.Contains(x.ProductId))
            .Select(x => x.ProductId)
            .ToList();

        if (unexpectedProductIds.Count > 0)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequests.QuantityMismatch)
                .WithData("MovementRequestId", request.Id)
                .WithData("UnexpectedProductIds", string.Join(",", unexpectedProductIds));
        }

        foreach (var line in expectedLines)
        {
            var receivedLine = model.Lines.SingleOrDefault(x => x.ProductId == line.ProductId);
            if (receivedLine == null)
            {
                throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequests.ReturnReceiveLineRequired)
                    .WithData("MovementRequestId", request.Id)
                    .WithData("ProductId", line.ProductId);
            }

            if (receivedLine.ReceivedQuantity < 0 ||
                receivedLine.DamagedQuantity < 0 ||
                receivedLine.LostQuantity < 0 ||
                receivedLine.ConsumedQuantity < 0)
            {
                throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequests.QuantityMismatch)
                    .WithData("MovementRequestId", request.Id)
                    .WithData("ProductId", line.ProductId);
            }

            var total = receivedLine.ReceivedQuantity +
                        receivedLine.DamagedQuantity +
                        receivedLine.LostQuantity +
                        receivedLine.ConsumedQuantity;

            if (total != line.Quantity)
            {
                throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequests.QuantityMismatch)
                    .WithData("MovementRequestId", request.Id)
                    .WithData("ProductId", line.ProductId)
                    .WithData("Expected", line.Quantity)
                    .WithData("Actual", total);
            }
        }
    }

    /// İade düzeltme notu oluşturmak için kullanılır.
    private static string BuildReturnAdjustmentNote(ReceiveMovementRequestLineModel line, string? receiveNote)
    {
        var reason = $"Return adjustment. Damaged={line.DamagedQuantity}; Lost={line.LostQuantity}; Consumed={line.ConsumedQuantity}.";
        if (!string.IsNullOrWhiteSpace(line.Note))
        {
            return $"{reason} {line.Note}";
        }

        return string.IsNullOrWhiteSpace(receiveNote) ? reason : $"{reason} {receiveNote}";
    }

    // Hareket talebi satırlarını getirir.
    /// Transfer edilecek satır verilerini getirmek için kullanılır.
    private async Task<List<MovementRequestLine>> GetLinesForTransferAsync(Guid movementRequestId)
    {
        var lines = await _movementRequestLineRepository.GetListAsync(x => x.MovementRequestId == movementRequestId);
        if (lines.Count == 0)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequestLines.NotFound)
                .WithData("MovementRequestId", movementRequestId);
        }

        return lines;
    }

    /// Güncelleme sırasında talep numarasının geçerliliğini doğrulamak için kullanılır.
    private async Task ValidateRequestNumberForUpdateAsync(MovementRequest existing, string requestNumber)
    {
        if (!string.IsNullOrWhiteSpace(requestNumber) && existing.RequestNumber != requestNumber)
        {
            await EnsureUniqueAsync(x => x.RequestNumber == requestNumber, existing.Id);
        }
    }

    /// Talep başlık referanslarını doğrulamak için kullanılır.
    private async Task ValidateHeaderReferencesAsync(
        Guid requestedByWorkerId,
        Guid SourceWarehouseId,
        Guid? TargetWarehouseId,
        Guid? requestedVehicleId,
        Guid? assignedTaskId)
    {
        await EnsureExistsInAsync(_workerRepository, requestedByWorkerId);
        await EnsureExistsInAsync(_warehouseRepository, SourceWarehouseId);
        ValidateMovementRoute(SourceWarehouseId, TargetWarehouseId, requestedVehicleId, assignedTaskId);
        
        if (HasValidId(TargetWarehouseId))
        {
            await EnsureExistsInAsync(_warehouseRepository, TargetWarehouseId.Value);
        }
        
        if (HasValidId(requestedVehicleId))
        {
            await ValidateRequestedVehicleAvailableAsync(requestedVehicleId.Value);
        }

        if (assignedTaskId.HasValue && assignedTaskId != Guid.Empty)
        {
            await EnsureExistsInAsync(_inventoryTaskRepository, assignedTaskId.Value);
        }
    }

    /// Değişen referans verilerini doğrulamak için kullanılır.
    private async Task ValidateChangedReferencesAsync(MovementRequest existing, UpdateMovementRequestModel model)
    {
        await ValidateHeaderReferencesAsync(
            model.RequestedByWorkerId,
            model.SourceWarehouseId,
            model.TargetWarehouseId,
            model.RequestedVehicleId,
            model.AssignedTaskId);
    }

    /// Hareket önceliğinin geçerliliğini doğrulamak için kullanılır.
    private async Task ValidatePriorityAsync(InventoryTrackingAutomation.Enums.MovementPriorityEnum priority)
    {
        await EnsureValidEnumAsync(
            priority,
            Settings.InventoryTrackingAutomationSettings.Movements.AllowedMovementPriorities);
    }

    /// Hareket statüsünün geçerliliğini doğrulamak için kullanılır.
    private async Task ValidateStatusAsync(InventoryTrackingAutomation.Enums.MovementStatusEnum status)
    {
        await EnsureValidEnumAsync(
            status,
            Settings.InventoryTrackingAutomationSettings.Movements.AllowedMovementStatuses);
    }

    /// Satırlardaki ürünlerin geçerliliğini doğrulamak için kullanılır.
    private async Task ValidateLineProductsAsync(CreateMovementRequestWithLinesModel model)
    {
        var productIds = model.Lines.Select(l => l.ProductId).Distinct().ToList();
        await EnsureAllExistInAsync(_productRepository, productIds);
    }

    /// İstenen aracın uygunluğunu doğrulamak için kullanılır.
    private async Task ValidateRequestedVehicleAvailableAsync(Guid requestedVehicleId)
    {
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

    /// Hareket rotasının geçerliliğini doğrulamak için kullanılır.
    private static void ValidateMovementRoute(
        Guid sourceWarehouseId,
        Guid? targetWarehouseId,
        Guid? requestedVehicleId,
        Guid? assignedTaskId)
    {
        EnsureVehicleRequested(requestedVehicleId);

        var hasTaskContext = HasValidId(assignedTaskId);
        if (!hasTaskContext)
        {
            EnsureTargetWarehouseRequested(targetWarehouseId);

            if (targetWarehouseId == sourceWarehouseId)
            {
                throw new BusinessException(InventoryTrackingAutomationErrorCodes.InventoryTransactions.InvalidLocationPair)
                    .WithData("SourceWarehouseId", sourceWarehouseId)
                    .WithData("TargetWarehouseId", targetWarehouseId);
            }
        }
    }

    /// Araç seçiminin yapıldığını doğrulamak için kullanılır.
    private static void EnsureVehicleRequested(Guid? requestedVehicleId)
    {
        if (MissingId(requestedVehicleId))
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequests.VehicleRequired);
        }
    }

    /// Hedef depo seçiminin yapıldığını doğrulamak için kullanılır.
    private static void EnsureTargetWarehouseRequested(Guid? targetWarehouseId)
    {
        if (MissingId(targetWarehouseId))
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequests.TargetRequired);
        }
    }

    /// Sevkiyat anında görev durumunun uygunluğunu doğrulamak için kullanılır.
    private async Task ValidateDispatchTaskStatusAsync(MovementRequest request)
    {
        if (MissingId(request.AssignedTaskId))
        {
            return;
        }

        var task = await _inventoryTaskRepository.FindAsync(request.AssignedTaskId.Value);
        if (task == null)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.InventoryTasks.NotFound)
                .WithData("InventoryTaskId", request.AssignedTaskId.Value);
        }

        // Kural: Sadece InProgress veya Draft olabilir.
        if (task.Status != TaskStatusEnum.InProgress && task.Status != TaskStatusEnum.Draft)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequests.DispatchNotAllowed)
                .WithData("MovementRequestId", request.Id)
                .WithData("InventoryTaskId", task.Id)
                .WithData("TaskStatus", task.Status)
                .WithData("ExpectedTaskStatus", $"{TaskStatusEnum.InProgress} or {TaskStatusEnum.Draft}");
        }

        // Eger hala Draft ise, depodan cikis yapildigi an gorevi "Devam Ediyor" (InProgress) statusune cekiyoruz.
        if (task.Status == TaskStatusEnum.Draft)
        {
            task.Status = TaskStatusEnum.InProgress;
            task.IsActive = true;
            await _inventoryTaskRepository.UpdateAsync(task, autoSave: true);
        }
    }

    /// Talebe uygun workflow ataması yapmak için kullanılır.
    private async Task<InventoryTrackingAutomation.Entities.Workflows.WorkflowInstance?> AssignWorkflowAsync(
        MovementRequest entity,
        Guid currentUserId)
    {
        // Eğer talep bir Göreve (Task) bağlıysa, daha yalın olan TaskMovementRequest akışını kullan.
        // Aksi halde (Depodan Depoya ise) standart MovementRequest akışını kullan.
        var workflowName = HasValidId(entity.AssignedTaskId)
            ? WorkflowDefinitionNames.TaskMovementRequest
            : WorkflowDefinitionNames.MovementRequest;

        var workflowDef = await _workflowDefinitionRepository.FindAsync(
            w => w.Name == workflowName && w.IsActive);
        
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

    private static bool HasValidId(Guid? id) => id.HasValue && id.Value != Guid.Empty;
    private static bool MissingId(Guid? id) => !id.HasValue || id.Value == Guid.Empty;

    /// İlk workflow adımı için bildirim yayınlamak için kullanılır.
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
