using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Interface.Workflows;
using InventoryTrackingAutomation.Events.Workflows;
using InventoryTrackingAutomation.Models.Movements;
using InventoryTrackingAutomation.Managers.Inventory;
using InventoryTrackingAutomation.Managers.Tasks;
using InventoryTrackingAutomation.Models.Inventory;
using InventoryTrackingAutomation.Workflows;
using Volo.Abp.EventBus.Local;
using Volo.Abp;
using Volo.Abp.Uow;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Movements;

// Hareket talebi domain manager'i - MovementRequest icin is kurallari, FK validasyonu ve workflow tetikleme.
public class MovementRequestManager : BaseManager<MovementRequest>
{
    protected override string AlreadyExistsErrorCode => MovementRequestExceptionCodes.RequestNumberNotUnique;

    private IWarehouseRepository _warehouseRepository => LazyGetRequiredService<IWarehouseRepository>();
    private IWorkerRepository _workerRepository => LazyGetRequiredService<IWorkerRepository>();
    private IVehicleRepository _vehicleRepository => LazyGetRequiredService<IVehicleRepository>();
    private IInventoryTaskRepository _inventoryTaskRepository => LazyGetRequiredService<IInventoryTaskRepository>();
    private IVehicleTaskRepository _vehicleTaskRepository => LazyGetRequiredService<IVehicleTaskRepository>();
    private Managers.Workflows.WorkflowManager _workflowManager => LazyGetRequiredService<Managers.Workflows.WorkflowManager>();
    private IWorkflowDefinitionRepository _workflowDefinitionRepository => LazyGetRequiredService<IWorkflowDefinitionRepository>();
    private IWorkflowInstanceRepository _workflowInstanceRepository => LazyGetRequiredService<IWorkflowInstanceRepository>();
    private ITaskLineRepository _taskLineRepository => LazyGetRequiredService<ITaskLineRepository>();
    private IVehicleTaskLineRepository _vehicleTaskLineRepository => LazyGetRequiredService<IVehicleTaskLineRepository>();
    private StockTransferManager _stockTransferManager => LazyGetRequiredService<StockTransferManager>();
    private StockAdjustmentManager _stockAdjustmentManager => LazyGetRequiredService<StockAdjustmentManager>();
    private VehicleTaskManager _vehicleTaskManager => LazyGetRequiredService<VehicleTaskManager>();
    private ILocalEventBus _localEventBus => LazyGetRequiredService<ILocalEventBus>();

    public MovementRequestManager(IMovementRequestRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    /// Yeni hareket talebi entity'si oluşturmak için kullanılır.
    public async Task<CreateMovementRequestModel> CreateAsync(CreateMovementRequestModel model)
    {
        await ValidateRequestNumberForCreateAsync(model.RequestNumber);
        await ValidateHeaderReferencesAsync(model.RequestedByWorkerId, model.VehicleTaskId);
        await ValidatePriorityAsync(model.Priority);

        return model;
    }

    /// <summary>
    /// Birden fazla hareket talebini toplu olarak doğrulamak için kullanılır.
    /// </summary>
    public async Task<System.Collections.Generic.List<CreateMovementRequestModel>> CreateManyAsync(System.Collections.Generic.List<CreateMovementRequestModel> models)
    {
        if (models.Count == 0)
        {
            return models;
        }

        var requestNumbers = models.Where(x => !string.IsNullOrWhiteSpace(x.RequestNumber)).Select(x => x.RequestNumber).ToList();
        if (requestNumbers.Any())
        {
            await EnsureUniqueBulkAsync(requestNumbers, x => x.RequestNumber);
        }

        var workerIds = models.Select(x => x.RequestedByWorkerId).Distinct().ToList();
        if (workerIds.Any())
        {
            await EnsureAllExistInAsync(_workerRepository, workerIds);
        }

        foreach (var priority in models.Select(x => x.Priority).Distinct())
        {
            await ValidatePriorityAsync(priority);
        }

        var vehicleTaskIds = models.Select(x => x.VehicleTaskId).Distinct().ToList();
        if (vehicleTaskIds.Any(x => x == Guid.Empty))
        {
            throw new BusinessException(VehicleTaskExceptionCodes.NotFound);
        }

        var vehicleTasks = await _vehicleTaskRepository.GetListAsync(x => vehicleTaskIds.Contains(x.Id));
        var vehicleTaskById = vehicleTasks.ToDictionary(x => x.Id);
        foreach (var vehicleTaskId in vehicleTaskIds)
        {
            if (!vehicleTaskById.TryGetValue(vehicleTaskId, out var vehicleTask))
            {
                throw new BusinessException(VehicleTaskExceptionCodes.NotFound);
            }

            if (vehicleTask.ReleasedAt.HasValue)
            {
                throw new BusinessException(GeneralExceptionCodes.InvalidOperation);
            }
        }

        var taskIds = vehicleTasks.Select(x => x.TaskId).Distinct().ToList();
        var tasks = await _inventoryTaskRepository.GetListAsync(x => taskIds.Contains(x.Id));
        var taskById = tasks.ToDictionary(x => x.Id);
        foreach (var taskId in taskIds)
        {
            if (!taskById.ContainsKey(taskId))
            {
                throw new BusinessException(InventoryTaskExceptionCodes.NotFound);
            }
        }

        var vehicleIds = vehicleTasks.Select(x => x.VehicleId).Distinct().ToList();
        var vehicles = await _vehicleRepository.GetListAsync(x => vehicleIds.Contains(x.Id));
        var vehicleById = vehicles.ToDictionary(x => x.Id);
        foreach (var vehicleId in vehicleIds)
        {
            if (!vehicleById.TryGetValue(vehicleId, out var vehicle))
            {
                throw new BusinessException(VehicleExceptionCodes.NotFound);
            }

            if (!vehicle.IsActive)
            {
                throw new BusinessException(GeneralExceptionCodes.InvalidOperation);
            }
        }

        var warehouseIds = new HashSet<Guid>();
        foreach (var vehicleTask in vehicleTasks)
        {
            var task = taskById[vehicleTask.TaskId];
            EnsureVehicleRequested(vehicleTask.VehicleId);
            CollectAndValidateTaskRoute(task, warehouseIds);
        }

        await EnsureAllExistInAsync(_warehouseRepository, warehouseIds);

        var transferLines = await _vehicleTaskLineRepository.GetListAsync(x => vehicleTaskIds.Contains(x.VehicleTaskId));
        var vehicleTaskIdsWithLines = transferLines.Select(x => x.VehicleTaskId).ToHashSet();
        foreach (var vehicleTaskId in vehicleTaskIds)
        {
            if (!vehicleTaskIdsWithLines.Contains(vehicleTaskId))
            {
                throw new BusinessException(GeneralExceptionCodes.InvalidOperation);
            }
        }

        return models;
    }

    /// Mevcut bir hareket talebini güncellemek için kullanılır.
    public async Task<UpdateMovementRequestModel> UpdateAsync(MovementRequest existing, UpdateMovementRequestModel model)
    {
        EnsureEditable(existing);
        await ValidateRequestNumberForUpdateAsync(existing, model.RequestNumber);
        await ValidateChangedReferencesAsync(model);
        await ValidatePriorityAsync(model.Priority);

        return model;
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
        var context = await EnsureOperationalContextAsync(request.Id);
        if (context.IsReturnFlow)
        {
            throw new BusinessException(MovementRequestExceptionCodes.DispatchNotAllowed);
        }
        EnsureStatus(request, MovementStatusEnum.Approved, MovementRequestExceptionCodes.DispatchNotAllowed);
        await ValidateVehicleAvailableAsync(context.VehicleId);
        await ValidateDispatchTaskStatusAsync(context);
        var lines = await GetVehicleTaskLinesForTransferAsync(context.VehicleTaskId);
        foreach (var lineContext in lines)
        {
            // Dispatch her surecte depo stokunu araca tasir; task/vehicle bilgisi join context'ten gelir.
            await _stockTransferManager.ExecuteAsync(new StockTransferModel
            {
                ProductId = lineContext.ProductId,
                Quantity = lineContext.Line.AllocatedQuantity,
                SourceLocationType = StockLocationTypeEnum.Warehouse,
                SourceLocationId = context.SourceWarehouseId,
                DestinationLocationType = StockLocationTypeEnum.Vehicle,
                DestinationLocationId = context.VehicleId,
                TransactionType = InventoryTransactionTypeEnum.WarehouseToVehicle,
                RelatedMovementRequestId = request.Id,
                PerformedByUserId = currentUserId,
                Note = dispatchNote
            });
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
        var context = await EnsureOperationalContextAsync(request.Id);
        EnsureStatus(request, MovementStatusEnum.Shipped, MovementRequestExceptionCodes.ReceiveNotAllowed);

        if (context.IsReturnFlow)
        {
            return await ReceiveTaskReturnAsync(request, context, model, currentUserId);
        }

        if (context.IsFieldOperation)
        {
            // Saha tesliminde stok aracta kalir; gorev kapaninca kontrollu iade akisi baslar.
            request.Status = MovementStatusEnum.Completed;
            return await Repository.UpdateAsync(request, autoSave: true);
        }

        if (MissingId(context.TargetWarehouseId))
        {
            throw new BusinessException(MovementRequestExceptionCodes.TargetRequired);
        }

        var targetWarehouseId = context.TargetWarehouseId.GetValueOrDefault();
        var lines = await GetVehicleTaskLinesForTransferAsync(context.VehicleTaskId);
        foreach (var lineContext in lines)
        {
            await _stockTransferManager.ExecuteAsync(new StockTransferModel
            {
                ProductId = lineContext.ProductId,
                Quantity = lineContext.Line.AllocatedQuantity,
                SourceLocationType = StockLocationTypeEnum.Vehicle,
                SourceLocationId = context.VehicleId,
                DestinationLocationType = StockLocationTypeEnum.Warehouse,
                DestinationLocationId = targetWarehouseId,
                TransactionType = InventoryTransactionTypeEnum.VehicleToWarehouse,
                RelatedMovementRequestId = request.Id,
                PerformedByUserId = currentUserId,
                Note = model.ReceiveNote
            });
        }

        request.Status = MovementStatusEnum.Completed;
        var saved = await Repository.UpdateAsync(request, autoSave: true);
        await CompleteWarehouseTransferTaskAsync(context);
        await _vehicleTaskManager.ReleaseForTaskVehicleAsync(context.TaskId, context.VehicleId);
        return saved;
    }

    /// Görev iadesi sürecinde malzemelerin teslim alınması için kullanılır.
    private async Task<MovementRequest> ReceiveTaskReturnAsync(
        MovementRequest request,
        MovementRequestOperationalContextModel context,
        ReceiveMovementRequestModel model,
        Guid currentUserId)
    {
        if (MissingId(context.TargetWarehouseId))
        {
            throw new BusinessException(MovementRequestExceptionCodes.TargetRequired);
        }

        var targetWarehouseId = context.TargetWarehouseId.GetValueOrDefault();
        var vehicleTaskLines = await GetVehicleTaskLinesForTransferAsync(context.VehicleTaskId);
        ValidateReturnReceiveInput(request, vehicleTaskLines, model);

        foreach (var vtl in vehicleTaskLines)
        {
            var receivedLine = model.Lines.Single(x => x.VehicleTaskLineId == vtl.Line.Id);
            if (receivedLine.ReceivedQuantity > 0)
            {
                await _stockTransferManager.ExecuteAsync(new StockTransferModel
                {
                    ProductId = vtl.ProductId,
                    Quantity = receivedLine.ReceivedQuantity,
                    SourceLocationType = StockLocationTypeEnum.Vehicle,
                    SourceLocationId = context.VehicleId,
                    DestinationLocationType = StockLocationTypeEnum.Warehouse,
                    DestinationLocationId = targetWarehouseId,
                    TransactionType = InventoryTransactionTypeEnum.VehicleToWarehouse,
                    RelatedMovementRequestId = request.Id,
                    PerformedByUserId = currentUserId,
                    Note = receivedLine.Note ?? model.ReceiveNote
                });
            }

            var adjustmentQuantity = receivedLine.DamagedQuantity + receivedLine.LostQuantity + receivedLine.ConsumedQuantity;
            if (adjustmentQuantity > 0)
            {
                await _stockAdjustmentManager.DecreaseAsync(new StockAdjustmentModel
                {
                    ProductId = vtl.ProductId,
                    Quantity = adjustmentQuantity,
                    SourceLocationType = StockLocationTypeEnum.Vehicle,
                    SourceLocationId = context.VehicleId,
                    RelatedMovementRequestId = request.Id,
                    PerformedByUserId = currentUserId,
                    Note = BuildReturnAdjustmentNote(receivedLine, model.ReceiveNote)
                });
            }

            // Uzlasma sonucunu VehicleTaskLine'a yazar.
            vtl.Line.ReceivedQuantity = receivedLine.ReceivedQuantity;
            vtl.Line.DamagedQuantity = receivedLine.DamagedQuantity;
            vtl.Line.LostQuantity = receivedLine.LostQuantity;
            vtl.Line.ConsumedQuantity = receivedLine.ConsumedQuantity;
            vtl.Line.ReceiveNote = receivedLine.Note;
            await _vehicleTaskLineRepository.UpdateAsync(vtl.Line, autoSave: true);
        }

        request.Status = MovementStatusEnum.Completed;
        var saved = await Repository.UpdateAsync(request, autoSave: true);
        await _vehicleTaskManager.ReleaseForTaskVehicleAsync(context.TaskId, context.VehicleId);
        return saved;
    }

    /// Transfer edilecek VehicleTaskLine verilerini getirmek için kullanılır.
    private async Task<List<VehicleTaskLineTransferContext>> GetVehicleTaskLinesForTransferAsync(Guid vehicleTaskId)
    {
        var lines = await _vehicleTaskLineRepository.GetByVehicleTaskIdAsync(vehicleTaskId);
        if (lines.Count == 0)
        {
            throw new BusinessException(VehicleTaskLineExceptionCodes.NotFound);
        }

        // ProductId VehicleTaskLine'da tekrar tutulmaz; hareket stok satiri icin TaskLine uzerinden cozulur.
        var taskLineIds = lines.Select(x => x.TaskLineId).Distinct().ToList();
        var taskLines = await _taskLineRepository.GetListAsync(x => taskLineIds.Contains(x.Id));
        var productByTaskLineId = taskLines.ToDictionary(x => x.Id, x => x.ProductId);
        var missingTaskLine = lines.FirstOrDefault(x => !productByTaskLineId.ContainsKey(x.TaskLineId));
        if (missingTaskLine != null)
        {
            throw new BusinessException(TaskLineExceptionCodes.NotFound);
        }

        return lines
            .Select(line => new VehicleTaskLineTransferContext(line, productByTaskLineId[line.TaskLineId]))
            .ToList();
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

        throw new BusinessException(MovementRequestExceptionCodes.InvalidStateTransition);
    }

    /// Talebin belirli bir statüde olup olmadığını doğrulamak için kullanılır.
    private static void EnsureStatus(MovementRequest request, MovementStatusEnum expectedStatus, string errorCode)
    {
        if (request.Status == expectedStatus)
        {
            return;
        }

        throw new BusinessException(errorCode);
    }

    /// MovementRequest icin normalize edilmis operasyon baglamini repository uzerinden getirir.
    private async Task<MovementRequestOperationalContextModel> EnsureOperationalContextAsync(Guid movementRequestId)
    {
        // Surec tipi MovementRequest.Type alanindan degil, Task.Type join'inden cozulur.
        var context = await ((IMovementRequestRepository)Repository).GetOperationalContextAsync(movementRequestId);
        if (context == null)
        {
            throw new BusinessException(MovementRequestExceptionCodes.NotFound);
        }

        return context;
    }

    /// İade alım giriş verilerini VehicleTaskLine bazlı doğrulamak için kullanılır.
    private static void ValidateReturnReceiveInput(
        MovementRequest request,
        IReadOnlyCollection<VehicleTaskLineTransferContext> expectedLines,
        ReceiveMovementRequestModel model)
    {
        if (model.Lines.Count == 0)
        {
            throw new BusinessException(
                MovementRequestExceptionCodes.ReturnReceiveLineRequired);
        }

        var duplicateIds = model.Lines
            .GroupBy(x => x.VehicleTaskLineId)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToList();

        if (duplicateIds.Count > 0)
        {
            throw new BusinessException(MovementRequestExceptionCodes.QuantityMismatch);
        }

        var expectedIds = expectedLines.Select(x => x.Line.Id).ToHashSet();
        var unexpectedIds = model.Lines
            .Where(x => !expectedIds.Contains(x.VehicleTaskLineId))
            .Select(x => x.VehicleTaskLineId)
            .ToList();

        if (unexpectedIds.Count > 0)
        {
            throw new BusinessException(MovementRequestExceptionCodes.QuantityMismatch);
        }

        foreach (var vtl in expectedLines)
        {
            var receivedLine = model.Lines.SingleOrDefault(x => x.VehicleTaskLineId == vtl.Line.Id);
            if (receivedLine == null)
            {
                throw new BusinessException(MovementRequestExceptionCodes.ReturnReceiveLineRequired);
            }

            if (receivedLine.ReceivedQuantity < 0 ||
                receivedLine.DamagedQuantity < 0 ||
                receivedLine.LostQuantity < 0 ||
                receivedLine.ConsumedQuantity < 0)
            {
                throw new BusinessException(MovementRequestExceptionCodes.QuantityMismatch);
            }

            var total = receivedLine.ReceivedQuantity +
                        receivedLine.DamagedQuantity +
                        receivedLine.LostQuantity +
                        receivedLine.ConsumedQuantity;

            if (total != vtl.Line.AllocatedQuantity)
            {
                throw new BusinessException(MovementRequestExceptionCodes.QuantityMismatch);
            }
        }
    }

    /// İade düzeltme notu oluşturmak için kullanılır.
    private static string BuildReturnAdjustmentNote(ReceiveMovementRequestVehicleTaskLineModel line, string? receiveNote)
    {
        var reason = $"Return adjustment. Damaged={line.DamagedQuantity}; Lost={line.LostQuantity}; Consumed={line.ConsumedQuantity}.";
        if (!string.IsNullOrWhiteSpace(line.Note))
        {
            return $"{reason} {line.Note}";
        }

        return string.IsNullOrWhiteSpace(receiveNote) ? reason : $"{reason} {receiveNote}";
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
    private async Task<VehicleTask> ValidateHeaderReferencesAsync(
        Guid requestedByWorkerId,
        Guid vehicleTaskId)
    {
        await EnsureExistsInAsync(_workerRepository, requestedByWorkerId);
        var vehicleTask = await EnsureVehicleTaskActiveAsync(vehicleTaskId);
        var task = await EnsureTaskExistsAsync(vehicleTask.TaskId);
        await ValidateTaskRouteAsync(task, vehicleTask.VehicleId);
        await ValidateVehicleAvailableAsync(vehicleTask.VehicleId);
        await EnsureVehicleTaskHasTransferLinesAsync(vehicleTask.Id);
        return vehicleTask;
    }

    /// Değişen referans verilerini doğrulamak için kullanılır.
    private async Task ValidateChangedReferencesAsync(UpdateMovementRequestModel model)
    {
        await ValidateHeaderReferencesAsync(
            model.RequestedByWorkerId,
            model.VehicleTaskId);
    }

    /// Hareket önceliğinin geçerliliğini doğrulamak için kullanılır.
    private async Task ValidatePriorityAsync(InventoryTrackingAutomation.Enums.MovementPriorityEnum priority)
    {
        await EnsureValidEnumAsync(
            priority,
            Settings.InventoryTrackingAutomationSettings.Movements.AllowedMovementPriorities);
    }

    /// İstenen aracın uygunluğunu doğrulamak için kullanılır.
    private async Task ValidateVehicleAvailableAsync(Guid vehicleId)
    {
        var vehicle = await _vehicleRepository.FindAsync(vehicleId);
        if (vehicle == null)
        {
            throw new Volo.Abp.BusinessException(VehicleExceptionCodes.NotFound);
        }

        if (!vehicle.IsActive)
        {
            throw new Volo.Abp.BusinessException(GeneralExceptionCodes.InvalidOperation);
        }
    }

    /// MovementRequest yalnizca mevcut ve aktif bir VehicleTask uzerinden acilabilir.
    private async Task<VehicleTask> EnsureVehicleTaskActiveAsync(Guid vehicleTaskId)
    {
        if (vehicleTaskId == Guid.Empty)
        {
            throw new BusinessException(VehicleTaskExceptionCodes.NotFound);
        }

        var vehicleTask = await _vehicleTaskRepository.FindAsync(vehicleTaskId);
        if (vehicleTask == null)
        {
            throw new BusinessException(VehicleTaskExceptionCodes.NotFound);
        }

        if (vehicleTask.ReleasedAt.HasValue)
        {
            throw new BusinessException(GeneralExceptionCodes.InvalidOperation);
        }

        return vehicleTask;
    }

    /// VehicleTaskLine olmadan hareket acilmasi stok transferini anlamsiz hale getirir.
    private async Task EnsureVehicleTaskHasTransferLinesAsync(Guid vehicleTaskId)
    {
        var lines = await _vehicleTaskLineRepository.GetByVehicleTaskIdAsync(vehicleTaskId);
        if (lines.Count == 0)
        {
            throw new BusinessException(GeneralExceptionCodes.InvalidOperation);
        }
    }

    /// Hareket rotasinin gecerliligini dogrulamak icin kullanilir.
    private static void CollectAndValidateTaskRoute(
        InventoryTrackingAutomation.Entities.Tasks.InventoryTask task,
        ISet<Guid> warehouseIds)
    {
        if (task.SourceWarehouseId == Guid.Empty)
        {
            throw new BusinessException(WarehouseExceptionCodes.NotFound);
        }

        warehouseIds.Add(task.SourceWarehouseId);

        if (task.Type != InventoryTaskTypeEnum.WarehouseTransfer)
        {
            return;
        }

        EnsureTargetWarehouseRequested(task.TargetWarehouseId);
        warehouseIds.Add(task.TargetWarehouseId!.Value);

        if (task.TargetWarehouseId == task.SourceWarehouseId)
        {
            throw new BusinessException(InventoryTransactionExceptionCodes.InvalidLocationPair);
        }
    }

    /// Hareket rotasinin gecerliligini dogrulamak icin kullanilir.
    private async Task ValidateTaskRouteAsync(
        InventoryTrackingAutomation.Entities.Tasks.InventoryTask task,
        Guid vehicleId)
    {
        // Arac her hareket icin VehicleTask uzerinden zorunludur.
        EnsureVehicleRequested(vehicleId);
        await EnsureExistsInAsync(_warehouseRepository, task.SourceWarehouseId);

        if (task.Type == InventoryTaskTypeEnum.WarehouseTransfer)
        {
            // Depo transferinde hedef depo zorunlu ve kaynak depodan farkli olmalidir.
            EnsureTargetWarehouseRequested(task.TargetWarehouseId);
            await EnsureExistsInAsync(_warehouseRepository, task.TargetWarehouseId!.Value);

            if (task.TargetWarehouseId == task.SourceWarehouseId)
            {
                throw new BusinessException(InventoryTransactionExceptionCodes.InvalidLocationPair);
            }
        }
    }

    /// Arac seciminin yapildigini dogrulamak icin kullanilir.
    private static void EnsureVehicleRequested(Guid vehicleId)
    {
        if (vehicleId == Guid.Empty)
        {
            throw new BusinessException(MovementRequestExceptionCodes.VehicleRequired);
        }
    }

    /// Hedef depo seciminin yapildigini dogrulamak icin kullanilir.
    private static void EnsureTargetWarehouseRequested(Guid? targetWarehouseId)
    {
        if (MissingId(targetWarehouseId))
        {
            throw new BusinessException(MovementRequestExceptionCodes.TargetRequired);
        }
    }

    /// Sevkiyat aninda task durumunun uygunlugunu dogrulamak icin kullanilir.
    private async Task ValidateDispatchTaskStatusAsync(MovementRequestOperationalContextModel context)
    {
        // Kural: Sadece Draft veya InProgress operasyonlar sevk edilebilir.
        if (context.TaskStatus != TaskStatusEnum.InProgress && context.TaskStatus != TaskStatusEnum.Draft)
        {
            throw new BusinessException(MovementRequestExceptionCodes.DispatchNotAllowed);
        }

        if (context.TaskStatus == TaskStatusEnum.Draft)
        {
            // Depodan cikis basladiginda operasyon isi InProgress olur.
            var task = await _inventoryTaskRepository.GetAsync(context.TaskId);
            task.Status = TaskStatusEnum.InProgress;
            await _inventoryTaskRepository.UpdateAsync(task, autoSave: true);
        }
    }

    /// Depo transferi receive tamamlandiginda operasyon isini kapatmak icin kullanilir.
    private async Task CompleteWarehouseTransferTaskAsync(MovementRequestOperationalContextModel context)
    {
        if (!context.IsWarehouseTransfer)
        {
            return;
        }

        var task = await _inventoryTaskRepository.GetAsync(context.TaskId);
        task.Status = TaskStatusEnum.Completed;
        task.EndDate ??= DateTime.UtcNow;
        await _inventoryTaskRepository.UpdateAsync(task, autoSave: true);
    }

    /// Task varligini repository uzerinden dogrulamak icin kullanilir.
    private async Task<InventoryTrackingAutomation.Entities.Tasks.InventoryTask> EnsureTaskExistsAsync(Guid taskId)
    {
        if (taskId == Guid.Empty)
        {
            throw new BusinessException(InventoryTaskExceptionCodes.NotFound);
        }

        var task = await _inventoryTaskRepository.FindAsync(taskId);
        if (task == null)
        {
            throw new BusinessException(InventoryTaskExceptionCodes.NotFound);
        }

        return task;
    }

    public async Task<InventoryTrackingAutomation.Entities.Workflows.WorkflowInstance?> AssignWorkflowAsync(
        MovementRequest entity,
        Guid currentUserId)
    {
        var vehicleTask = await EnsureVehicleTaskActiveAsync(entity.VehicleTaskId);
        var taskId = vehicleTask.TaskId;
        // Workflow secimi MovementRequest.Type alanindan degil, Task.Type alanindan yapilir.
        var task = await EnsureTaskExistsAsync(taskId);
        var workflowName = task.Type == InventoryTaskTypeEnum.FieldOperation
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

    private sealed record VehicleTaskLineTransferContext(VehicleTaskLine Line, Guid ProductId);

    /// İlk workflow adımı için bildirim yayınlamak için kullanılır.
    public Task PublishInitialWorkflowStepAssignedAsync(InventoryTrackingAutomation.Entities.Workflows.WorkflowInstance? workflowInstance)
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

