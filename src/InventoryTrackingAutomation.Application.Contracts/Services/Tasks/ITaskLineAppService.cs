using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Tasks;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Tasks;

/// <summary>
/// Görev kalemi uygulama servisi kontratı.
/// </summary>
public interface ITaskLineAppService : IApplicationService
{
    Task<TaskLineDto> GetAsync(Guid id);
    Task<List<TaskLineDto>> GetByTaskAsync(Guid taskId);
    Task<TaskLineDto> CreateForTaskAsync(Guid taskId, CreateTaskLineDto input);
    Task<TaskLineDto> UpdateAsync(Guid taskId, Guid lineId, UpdateTaskLineDto input);
    Task DeleteAsync(Guid taskId, Guid lineId);
}
