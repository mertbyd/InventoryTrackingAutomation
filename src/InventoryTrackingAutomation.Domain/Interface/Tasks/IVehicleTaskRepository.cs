using InventoryTrackingAutomation.Models.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;

namespace InventoryTrackingAutomation.Interface.Tasks;

/// <summary>
/// VehicleTask entity'si icin repository arayuzu.
/// </summary>
public interface IVehicleTaskRepository : IBaseRepository<VehicleTask>
{
    Task<List<TaskVehicleModel>> GetTaskVehiclesByTaskIdAsync(Guid inventoryTaskId);
}

