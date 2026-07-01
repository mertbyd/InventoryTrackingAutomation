using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;

namespace InventoryTrackingAutomation.Interface.Tasks;

/// <summary>
/// VehicleTaskLine entity'si icin repository arayuzu.
/// </summary>
public interface IVehicleTaskLineRepository : IBaseRepository<VehicleTaskLine>
{
    Task<List<VehicleTaskLine>> GetByVehicleTaskIdAsync(Guid vehicleTaskId);
    Task<List<VehicleTaskLine>> GetByTaskLineIdsAsync(IReadOnlyCollection<Guid> taskLineIds);
    Task<VehicleTaskLine?> FindByVehicleTaskAndTaskLineAsync(Guid vehicleTaskId, Guid taskLineId);
    Task<int> GetAllocatedQuantityByTaskLineIdAsync(Guid taskLineId, Guid? excludedVehicleTaskLineId = null);
    Task<List<InventoryTrackingAutomation.Models.Tasks.VehicleTaskLineWithProductModel>> GetTransferContextsByVehicleTaskIdAsync(Guid vehicleTaskId);
}
