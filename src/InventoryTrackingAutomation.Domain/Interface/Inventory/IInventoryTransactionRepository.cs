using InventoryTrackingAutomation.Entities.Inventory;

namespace InventoryTrackingAutomation.Interface.Inventory;

/// <summary>
/// InventoryTransaction entity'si icin repository arayuzu.
/// </summary>
public interface IInventoryTransactionRepository : IBaseRepository<InventoryTransaction>
{
    System.Threading.Tasks.Task<System.Collections.Generic.List<InventoryTrackingAutomation.Models.Movements.TaskVehicleReturnLineModel>> GetVehicleReturnLinesAsync(System.Collections.Generic.HashSet<System.Guid> movementIds, System.Guid vehicleId);
    System.Threading.Tasks.Task<System.Guid?> GetLastSourceWarehouseIdAsync(System.Collections.Generic.HashSet<System.Guid> movementIds, System.Guid vehicleId);
}
