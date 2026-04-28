using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Masters;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Masters;

/// <summary>
/// Lokasyon uygulama servisi kontratı — 6 temel CRUD operasyonunu tanımlar.
/// </summary>
public interface IWarehouseAppService : IApplicationService
{
    /// <summary> Id'ye göre tek lokasyon getirir. </summary>
    Task<WarehouseDto> GetAsync(Guid id);

    /// <summary> Lokasyonları sayfalı listeler. </summary>
    Task<PagedResultDto<WarehouseDto>> GetListAsync(PagedResultRequestDto input);

    /// <summary> Yeni lokasyon oluşturur. </summary>
    Task<WarehouseDto> CreateAsync(CreateWarehouseDto input);

    /// <summary> Birden fazla lokasyonu toplu oluşturur. </summary>
    Task<List<WarehouseDto>> CreateManyAsync(List<CreateWarehouseDto> inputs);

    /// <summary> Lokasyonu günceller. </summary>
    Task<WarehouseDto> UpdateAsync(Guid id, UpdateWarehouseDto input);

    /// <summary> Lokasyonu soft delete ile siler. </summary>
    Task DeleteAsync(Guid id);
}
