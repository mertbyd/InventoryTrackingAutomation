using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Interface.Inventory;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Managers.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Managers.Movements;

/// <summary>
/// Gorev kapanisinda aractaki kalan malzemeler icin kontrollu iade talebi uretir.
/// </summary>
public class TaskReturnRequestManager : DomainService
{
    private readonly IInventoryTaskRepository _taskRepository;
    private readonly IVehicleTaskRepository _vehicleTaskRepository;
    private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
    private readonly IMovementRequestRepository _movementRequestRepository;
    private readonly IMovementRequestLineRepository _movementRequestLineRepository;
    private readonly VehicleTaskManager _vehicleTaskManager;

    public TaskReturnRequestManager(
        IInventoryTaskRepository taskRepository,
        IVehicleTaskRepository vehicleTaskRepository,
        IInventoryTransactionRepository inventoryTransactionRepository,
        IMovementRequestRepository movementRequestRepository,
        IMovementRequestLineRepository movementRequestLineRepository,
        VehicleTaskManager vehicleTaskManager)
    {
        _taskRepository = taskRepository;
        _vehicleTaskRepository = vehicleTaskRepository;
        _inventoryTransactionRepository = inventoryTransactionRepository;
        _movementRequestRepository = movementRequestRepository;
        _movementRequestLineRepository = movementRequestLineRepository;
        _vehicleTaskManager = vehicleTaskManager;
    }

    /// Görev için iade talepleri oluşturmak için kullanılır.
    [UnitOfWork]
    public async Task CreateReturnRequestsForTaskAsync(
        Guid taskId,
        Guid? changedByUserId,
        Guid? changedByWorkerId)
    {
        var task = await _taskRepository.FindAsync(taskId);
        if (task == null)
        {
            return;
        }

        var activeAssignments = await _vehicleTaskRepository.GetListAsync(x =>
            x.InventoryTaskId == taskId &&
            x.IsActive);

        foreach (var assignment in activeAssignments)
        {
            var returnLines = await GetTaskVehicleReturnLinesAsync(taskId, assignment.VehicleId);
            if (returnLines.Count == 0)
            {
                await _vehicleTaskManager.ReleaseForTaskVehicleAsync(taskId, assignment.VehicleId);
                continue;
            }

            var existingReturnRequest = await _movementRequestRepository.FindAsync(x =>
                x.Type == MovementRequestTypeEnum.TaskReturnToWarehouse &&
                x.AssignedTaskId == taskId &&
                x.RequestedVehicleId == assignment.VehicleId &&
                x.Status != MovementStatusEnum.Completed &&
                x.Status != MovementStatusEnum.Rejected &&
                x.Status != MovementStatusEnum.Cancelled);

            if (existingReturnRequest != null)
            {
                continue;
            }

            var returnWarehouseId = task.ReturnWarehouseId ?? await ResolveLastSourceWarehouseAsync(taskId, assignment.VehicleId);
            var request = new MovementRequest(GuidGenerator.Create())
            {
                RequestNumber = GenerateRequestNumber(),
                RequestedByWorkerId = changedByWorkerId ?? assignment.DriverWorkerId,
                SourceWarehouseId = returnWarehouseId,
                TargetWarehouseId = returnWarehouseId,
                RequestedVehicleId = assignment.VehicleId,
                AssignedTaskId = taskId,
                Type = MovementRequestTypeEnum.TaskReturnToWarehouse,
                Status = MovementStatusEnum.Shipped,
                Priority = MovementPriorityEnum.Normal,
                RequestNote = $"Task return request for {task.Code}",
                PlannedDate = DateTime.UtcNow,
                WorkflowInstanceId = null
            };

            var insertedRequest = await _movementRequestRepository.InsertAsync(request, autoSave: true);
            var lineEntities = returnLines.Select(line => new MovementRequestLine(GuidGenerator.Create())
            {
                MovementRequestId = insertedRequest.Id,
                ProductId = line.ProductId,
                Quantity = line.Quantity
            }).ToList();

            await _movementRequestLineRepository.InsertManyAsync(lineEntities, autoSave: true);
        }
    }

    /// En son kaynak depoyu çözümlemek için kullanılır.
    private async Task<Guid> ResolveLastSourceWarehouseAsync(Guid taskId, Guid vehicleId)
    {
        var lastTransaction = (await _inventoryTransactionRepository.GetListAsync(x =>
            x.RelatedTaskId == taskId &&
            x.TargetLocationType == StockLocationTypeEnum.Vehicle &&
            x.TargetLocationId == vehicleId &&
            x.TransactionType == InventoryTransactionTypeEnum.WarehouseToVehicle))
            .OrderByDescending(x => x.OccurredAt)
            .FirstOrDefault();

        if (lastTransaction?.SourceLocationType == StockLocationTypeEnum.Warehouse &&
            lastTransaction.SourceLocationId.HasValue)
        {
            return lastTransaction.SourceLocationId.Value;
        }

        throw new BusinessException(InventoryTrackingAutomationErrorCodes.General.InvalidOperation)
            .WithData("TaskId", taskId)
            .WithData("VehicleId", vehicleId)
            .WithData("Reason", "Cannot resolve return warehouse");
    }

    /// Görev aracındaki iade satırlarını getirmek için kullanılır.
    private async Task<IReadOnlyList<TaskVehicleReturnLine>> GetTaskVehicleReturnLinesAsync(Guid taskId, Guid vehicleId)
    {
        var transactions = await _inventoryTransactionRepository.GetListAsync(x =>
            x.RelatedTaskId == taskId &&
            (
                (x.TransactionType == InventoryTransactionTypeEnum.WarehouseToVehicle &&
                 x.TargetLocationType == StockLocationTypeEnum.Vehicle &&
                 x.TargetLocationId == vehicleId) ||
                (x.TransactionType == InventoryTransactionTypeEnum.VehicleToWarehouse &&
                 x.SourceLocationType == StockLocationTypeEnum.Vehicle &&
                 x.SourceLocationId == vehicleId) ||
                (x.TransactionType == InventoryTransactionTypeEnum.Adjustment &&
                 x.SourceLocationType == StockLocationTypeEnum.Vehicle &&
                 x.SourceLocationId == vehicleId)
            ));

        return transactions
            .GroupBy(x => x.ProductId)
            .Select(group => new TaskVehicleReturnLine(
                group.Key,
                group.Sum(x => x.TransactionType == InventoryTransactionTypeEnum.WarehouseToVehicle
                    ? x.Quantity
                    : -x.Quantity)))
            .Where(x => x.Quantity > 0)
            .ToList();
    }

    /// Talep numarası üretmek için kullanılır.
    private string GenerateRequestNumber()
    {
        var suffix = GuidGenerator.Create().ToString("N")[..8].ToUpperInvariant();
        return $"RET-{DateTime.UtcNow:yyyyMMddHHmmss}-{suffix}";
    }

    private sealed record TaskVehicleReturnLine(Guid ProductId, int Quantity);
}
