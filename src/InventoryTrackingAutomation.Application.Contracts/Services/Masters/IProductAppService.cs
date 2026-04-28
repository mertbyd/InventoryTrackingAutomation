using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Dtos.Stock;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Masters;

/// <summary>
/// Ürün uygulama servisi kontratı — 6 temel CRUD operasyonunu tanımlar.
/// </summary>
public interface IProductAppService : IApplicationService
{
    /// <summary> Id'ye göre tek ürün getirir. </summary>
    Task<ProductDto> GetAsync(Guid id);

    /// <summary> Ürünleri sayfalı listeler. </summary>
    Task<PagedResultDto<ProductDto>> GetListAsync(PagedResultRequestDto input);

    /// <summary> Urunun depo/arac/gorev bazli stok ozetini getirir. </summary>
    Task<ProductStockSummaryDto> GetStockSummaryAsync(Guid id);

    /// <summary> Yeni ürün oluşturur. </summary>
    Task<ProductDto> CreateAsync(CreateProductDto input);

    /// <summary> Birden fazla ürünü toplu oluşturur. </summary>
    Task<List<ProductDto>> CreateManyAsync(List<CreateProductDto> inputs);

    /// <summary> Ürünü günceller. </summary>
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto input);

    /// <summary> Ürünü soft delete ile siler. </summary>
    Task DeleteAsync(Guid id);
}
