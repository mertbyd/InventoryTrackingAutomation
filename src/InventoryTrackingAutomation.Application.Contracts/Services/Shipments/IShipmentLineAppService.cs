using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Shipments;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Shipments;

/// <summary>
/// Sevkiyat satırı uygulama servisi kontratı — 6 temel CRUD operasyonunu tanımlar.
/// </summary>
public interface IShipmentLineAppService : IApplicationService
{
    /// <summary> Id'ye göre tek sevkiyat satırı getirir. </summary>
    Task<ShipmentLineDto> GetAsync(Guid id);

    /// <summary> Sevkiyat satırlarını sayfalı listeler. </summary>
    Task<PagedResultDto<ShipmentLineDto>> GetListAsync(PagedResultRequestDto input);

    /// <summary> Yeni sevkiyat satırı oluşturur. </summary>
    Task<ShipmentLineDto> CreateAsync(CreateShipmentLineDto input);

    /// <summary> Birden fazla sevkiyat satırını toplu oluşturur. </summary>
    Task<List<ShipmentLineDto>> CreateManyAsync(List<CreateShipmentLineDto> inputs);

    /// <summary> Sevkiyat satırını günceller. </summary>
    Task<ShipmentLineDto> UpdateAsync(Guid id, UpdateShipmentLineDto input);

    /// <summary> Sevkiyat satırını soft delete ile siler. </summary>
    Task DeleteAsync(Guid id);
}
