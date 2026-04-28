using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Tasks;

/// <summary>
/// Envanter gorevi uygulama servisi kontrati.
/// </summary>
public interface IInventoryTaskAppService : IApplicationService
{
    Task<InventoryTaskDto> GetAsync(Guid id);
    Task<PagedResultDto<InventoryTaskDto>> GetListAsync(PagedResultRequestDto input);
    Task<List<TaskVehicleDto>> GetVehiclesAsync(Guid id);
    Task<List<TaskInventoryDto>> GetInventoryAsync(Guid id);
    Task<InventoryTaskDto> CreateAsync(CreateInventoryTaskDto input);
    Task<List<InventoryTaskDto>> CreateManyAsync(List<CreateInventoryTaskDto> inputs);
    Task<InventoryTaskDto> UpdateAsync(Guid id, UpdateInventoryTaskDto input);
    Task<InventoryTaskDto> CompleteAsync(Guid id);
    Task<InventoryTaskDto> CancelAsync(Guid id);
    Task DeleteAsync(Guid id);
}
