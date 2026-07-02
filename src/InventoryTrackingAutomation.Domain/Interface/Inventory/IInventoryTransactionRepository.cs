using InventoryTrackingAutomation.Models.Movements;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Inventory;

namespace InventoryTrackingAutomation.Interface.Inventory;

/// <summary>
/// InventoryTransaction entity'si icin repository arayuzu.
/// </summary>
public interface IInventoryTransactionRepository : IBaseRepository<InventoryTransaction>
{
    Task<List<TaskVehicleReturnLineModel>> GetVehicleReturnLinesAsync(HashSet<Guid> movementIds, Guid vehicleId);
    Task<Guid?> GetLastSourceWarehouseIdAsync(HashSet<Guid> movementIds, Guid vehicleId);
}

