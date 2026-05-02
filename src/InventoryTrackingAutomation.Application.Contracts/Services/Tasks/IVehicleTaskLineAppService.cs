using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Tasks;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Tasks;

/// <summary>
/// Araç-görev kalemi uygulama servisi kontratı.
/// </summary>
public interface IVehicleTaskLineAppService : IApplicationService
{
    Task<VehicleTaskLineDto> GetAsync(Guid id);
    Task<List<VehicleTaskLineDto>> GetByVehicleTaskAsync(Guid vehicleTaskId);
    Task<VehicleTaskLineDto> CreateForVehicleTaskAsync(Guid vehicleTaskId, CreateVehicleTaskLineDto input);
    Task<VehicleTaskLineDto> UpdateAsync(Guid vehicleTaskId, Guid lineId, UpdateVehicleTaskLineDto input);
    Task DeleteAsync(Guid vehicleTaskId, Guid lineId);
}
