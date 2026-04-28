using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Dtos.Inventory;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Masters;

/// <summary>
/// Araç uygulama servisi kontratı — 6 temel CRUD operasyonunu tanımlar.
/// </summary>
public interface IVehicleAppService : IApplicationService
{
    /// <summary> Id'ye göre tek araç getirir. </summary>
    Task<VehicleDto> GetAsync(Guid id);

    /// <summary> Araçları sayfalı listeler. </summary>
    Task<PagedResultDto<VehicleDto>> GetListAsync(PagedResultRequestDto input);

    /// <summary> Aracin uzerindeki envanterleri getirir. </summary>
    Task<List<VehicleInventoryDto>> GetInventoriesAsync(Guid id);

    /// <summary> Yeni araç oluşturur. </summary>
    Task<VehicleDto> CreateAsync(CreateVehicleDto input);

    /// <summary> Birden fazla aracı toplu oluşturur. </summary>
    Task<List<VehicleDto>> CreateManyAsync(List<CreateVehicleDto> inputs);

    /// <summary> Aracı günceller. </summary>
    Task<VehicleDto> UpdateAsync(Guid id, UpdateVehicleDto input);

    /// <summary> Aracı soft delete ile siler. </summary>
    Task DeleteAsync(Guid id);
}
