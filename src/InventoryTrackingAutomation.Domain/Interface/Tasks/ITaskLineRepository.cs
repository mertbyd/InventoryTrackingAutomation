using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;

namespace InventoryTrackingAutomation.Interface.Tasks;

/// <summary>
/// TaskLine entity'si icin repository arayuzu.
/// </summary>
public interface ITaskLineRepository : IBaseRepository<TaskLine>
{
    Task<List<TaskLine>> GetByTaskIdAsync(Guid taskId);
    Task<TaskLine?> FindByTaskAndProductAsync(Guid taskId, Guid productId);
}
