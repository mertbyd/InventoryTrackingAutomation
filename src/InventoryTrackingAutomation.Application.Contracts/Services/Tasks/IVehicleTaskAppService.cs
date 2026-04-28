using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Tasks;

/// <summary>
/// Arac-gorev atamasi uygulama servisi kontrati.
/// </summary>
public interface IVehicleTaskAppService : IApplicationService
{
    Task<VehicleTaskDto> GetAsync(Guid id);
    Task<PagedResultDto<VehicleTaskDto>> GetListAsync(PagedResultRequestDto input);
    Task<VehicleTaskDto> CreateAsync(CreateVehicleTaskDto input);
    Task<List<VehicleTaskDto>> CreateManyAsync(List<CreateVehicleTaskDto> inputs);
    Task<VehicleTaskDto> UpdateAsync(Guid id, UpdateVehicleTaskDto input);
    Task DeleteAsync(Guid id);
}
