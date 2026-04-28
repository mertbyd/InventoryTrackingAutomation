using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Workflows;

namespace InventoryTrackingAutomation.Managers.Workflows.Approvers;

// Source/Target Warehouse manager strategy'leri için ortak çözümleme: MovementRequest → Warehouse → ManagerWorker → User.
internal static class WarehouseApproverResolver
{
    public static async Task<Guid?> ResolveAsync(
        ApproverContext context,
        bool useSourceWarehouse,
        IMovementRequestRepository movementRequestRepository,
        IWarehouseRepository warehouseRepository,
        IWorkerRepository workerRepository)
    {
        if (context.EntityType != WorkflowEntityTypes.MovementRequest)
        {
            return null;
        }

        var request = await movementRequestRepository.FindAsync(context.EntityId);
        if (request == null)
        {
            return null;
        }

        var warehouseId = useSourceWarehouse ? request.SourceWarehouseId : request.TargetWarehouseId;
        if (warehouseId == Guid.Empty)
        {
            return null;
        }

        var warehouse = await warehouseRepository.FindAsync(warehouseId);
        if (warehouse?.ManagerWorkerId == null)
        {
            return null;
        }

        var manager = await workerRepository.FindAsync(warehouse.ManagerWorkerId.Value);
        return manager?.UserId;
    }
}
