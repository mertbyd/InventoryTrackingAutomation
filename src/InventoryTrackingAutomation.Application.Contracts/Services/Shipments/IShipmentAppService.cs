using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Shipments;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Shipments;

/// <summary>
/// Sevkiyat uygulama servisi kontratı — 6 temel CRUD operasyonunu tanımlar.
/// </summary>
public interface IShipmentAppService : IApplicationService
{
    /// <summary> Id'ye göre tek sevkiyat getirir. </summary>
    Task<ShipmentDto> GetAsync(Guid id);

    /// <summary> Sevkiyatları sayfalı listeler. </summary>
    Task<PagedResultDto<ShipmentDto>> GetListAsync(PagedResultRequestDto input);

    /// <summary> Yeni sevkiyat oluşturur. </summary>
    Task<ShipmentDto> CreateAsync(CreateShipmentDto input);

    /// <summary> Birden fazla sevkiyatı toplu oluşturur. </summary>
    Task<List<ShipmentDto>> CreateManyAsync(List<CreateShipmentDto> inputs);

    /// <summary> Sevkiyatı günceller. </summary>
    Task<ShipmentDto> UpdateAsync(Guid id, UpdateShipmentDto input);

    /// <summary> Sevkiyatı soft delete ile siler. </summary>
    Task DeleteAsync(Guid id);
}
