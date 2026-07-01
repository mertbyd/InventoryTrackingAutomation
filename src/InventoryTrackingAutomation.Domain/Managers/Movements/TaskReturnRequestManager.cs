using System;
using InventoryTrackingAutomation.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Interface.Inventory;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Managers.Tasks;
using Volo.Abp;
using Volo.Abp.Uow;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Movements;

/// <summary>
/// Gorev kapanisinda aractaki kalan malzemeler icin kontrollu iade talebi uretir.
/// </summary>
public class TaskReturnRequestManager : InventoryTrackingAutomationDomainService
{
    public TaskReturnRequestManager(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IInventoryTaskRepository _taskRepository => LazyGetRequiredService<IInventoryTaskRepository>();
    private IVehicleTaskRepository _vehicleTaskRepository => LazyGetRequiredService<IVehicleTaskRepository>();
    private IInventoryTransactionRepository _inventoryTransactionRepository => LazyGetRequiredService<IInventoryTransactionRepository>();
    private IMovementRequestRepository _movementRequestRepository => LazyGetRequiredService<IMovementRequestRepository>();
    private ITaskLineRepository _taskLineRepository => LazyGetRequiredService<ITaskLineRepository>();
    private IVehicleTaskLineRepository _vehicleTaskLineRepository => LazyGetRequiredService<IVehicleTaskLineRepository>();
    private VehicleTaskManager _vehicleTaskManager => LazyGetRequiredService<VehicleTaskManager>();

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

        if (task.Type != InventoryTaskTypeEnum.FieldOperation)
        {
            return;
        }

        var activeAssignments = await _vehicleTaskRepository.GetListAsync(x =>
            x.TaskId == taskId &&
            !x.ReleasedAt.HasValue);

        foreach (var assignment in activeAssignments)
        {
            var returnLines = await GetTaskVehicleReturnLinesAsync(taskId, assignment.Id, assignment.VehicleId);
            if (returnLines.Count == 0)
            {
                await _vehicleTaskManager.ReleaseForTaskVehicleAsync(taskId, assignment.VehicleId);
                continue;
            }

            var existingReturnRequest = await _movementRequestRepository.FindAsync(x =>
                x.VehicleTaskId == assignment.Id &&
                x.ParentMovementRequestId != null &&
                x.Status != MovementStatusEnum.Completed &&
                x.Status != MovementStatusEnum.Rejected &&
                x.Status != MovementStatusEnum.Cancelled);

            if (existingReturnRequest != null)
            {
                continue;
            }

            var returnWarehouseId = task.ReturnWarehouseId ?? await ResolveLastSourceWarehouseAsync(taskId, assignment.Id, assignment.VehicleId);
            var parentMovementId = await _movementRequestRepository.FindLatestMainMovementIdAsync(assignment.Id);
            if (!parentMovementId.HasValue)
            {
                throw new BusinessException(GeneralExceptionCodes.InvalidOperation);
            }

            // Doner iade talebini olustur (header). Satirlar VehicleTaskLine'da tutulur.
            var request = new MovementRequest(GuidGenerator.Create())
            {
                RequestNumber = GenerateRequestNumber(),
                RequestedByWorkerId = changedByWorkerId ?? assignment.ResponsibleWorkerId,
                VehicleTaskId = assignment.Id,
                ParentMovementRequestId = parentMovementId.Value,
                Status = MovementStatusEnum.Shipped,
                Priority = MovementPriorityEnum.Normal,
                RequestNote = $"Task return request for {task.Code}",
                PlannedDate = DateTime.UtcNow,
                WorkflowInstanceId = null
            };

            await _movementRequestRepository.InsertAsync(request, autoSave: true);

            // VehicleTaskLine kayitlarini iade miktarlariyla guncelle/olustur.
            await EnsureVehicleTaskLinesAsync(taskId, assignment.Id, returnLines);
        }
    }

    /// Iade akisi icin VehicleTaskLine kayitlarini guvenceye alir; eksikse olusturur.
    private async Task EnsureVehicleTaskLinesAsync(Guid taskId, Guid vehicleTaskId, IReadOnlyList<InventoryTrackingAutomation.Models.Movements.TaskVehicleReturnLineModel> returnLines)
    {
        foreach (var line in returnLines)
        {
            // TaskLine urunu temsil eder; VehicleTaskLine sadece bu gorev kalemine arac tahsisini baglar.
            var taskLine = await _taskLineRepository.FindByTaskAndProductAsync(taskId, line.ProductId);
            if (taskLine == null)
            {
                taskLine = new TaskLine(GuidGenerator.Create())
                {
                    TaskId = taskId,
                    ProductId = line.ProductId,
                    Quantity = line.Quantity
                };
                taskLine = await _taskLineRepository.InsertAsync(taskLine, autoSave: true);
            }

            var existing = await _vehicleTaskLineRepository.FindByVehicleTaskAndTaskLineAsync(vehicleTaskId, taskLine.Id);
            var targetVehicleLineQuantity = Math.Max(existing?.AllocatedQuantity ?? 0, line.Quantity);
            var allocatedExceptCurrent = await _vehicleTaskLineRepository.GetAllocatedQuantityByTaskLineIdAsync(taskLine.Id, existing?.Id);
            var requiredTaskQuantity = allocatedExceptCurrent + targetVehicleLineQuantity;
            if (requiredTaskQuantity > taskLine.Quantity)
            {
                taskLine.Quantity = requiredTaskQuantity;
                await _taskLineRepository.UpdateAsync(taskLine, autoSave: true);
            }

            if (existing != null)
            {
                // Daha onceden olusturulmus; iade edilecek fiili miktar daha yuksekse arac tahsisini guncelle.
                if (existing.AllocatedQuantity < line.Quantity)
                {
                    existing.AllocatedQuantity = line.Quantity;
                    await _vehicleTaskLineRepository.UpdateAsync(existing, autoSave: true);
                }
                continue;
            }

            var vtl = new VehicleTaskLine(GuidGenerator.Create())
            {
                VehicleTaskId = vehicleTaskId,
                TaskLineId = taskLine.Id,
                AllocatedQuantity = line.Quantity
            };
            await _vehicleTaskLineRepository.InsertAsync(vtl, autoSave: true);
        }
    }

    /// En son kaynak depoyu çözümlemek için kullanılır.
    private async Task<Guid> ResolveLastSourceWarehouseAsync(Guid taskId, Guid vehicleTaskId, Guid vehicleId)
    {
        var movementIds = (await _movementRequestRepository.GetListAsync(x =>
                x.VehicleTaskId == vehicleTaskId))
            .Select(x => x.Id)
            .ToHashSet();

        var sourceWarehouseId = await _inventoryTransactionRepository.GetLastSourceWarehouseIdAsync(movementIds, vehicleId);
        if (sourceWarehouseId.HasValue)
        {
            return sourceWarehouseId.Value;
        }
        
        throw new BusinessException(GeneralExceptionCodes.InvalidOperation);
    }

    /// Görev aracındaki iade satırlarını getirmek için kullanılır.
    private async Task<IReadOnlyList<InventoryTrackingAutomation.Models.Movements.TaskVehicleReturnLineModel>> GetTaskVehicleReturnLinesAsync(Guid taskId, Guid vehicleTaskId, Guid vehicleId)
    {
        var movementIds = (await _movementRequestRepository.GetListAsync(x =>
                x.VehicleTaskId == vehicleTaskId))
            .Select(x => x.Id)
            .ToHashSet();

        return await _inventoryTransactionRepository.GetVehicleReturnLinesAsync(movementIds, vehicleId);
    }

    /// Talep numarası üretmek için kullanılır.
    private string GenerateRequestNumber()
    {
        var suffix = GuidGenerator.Create().ToString("N")[..8].ToUpperInvariant();
        return $"RET-{DateTime.UtcNow:yyyyMMddHHmmss}-{suffix}";
    }
}

