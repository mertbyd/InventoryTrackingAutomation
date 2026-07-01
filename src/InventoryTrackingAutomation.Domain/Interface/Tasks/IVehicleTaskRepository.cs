using InventoryTrackingAutomation.Entities.Tasks;

namespace InventoryTrackingAutomation.Interface.Tasks;

/// <summary>
/// VehicleTask entity'si icin repository arayuzu.
/// </summary>
public interface IVehicleTaskRepository : IBaseRepository<VehicleTask>
{
    System.Threading.Tasks.Task<System.Collections.Generic.List<InventoryTrackingAutomation.Models.Tasks.TaskVehicleModel>> GetTaskVehiclesByTaskIdAsync(System.Guid inventoryTaskId);
}
