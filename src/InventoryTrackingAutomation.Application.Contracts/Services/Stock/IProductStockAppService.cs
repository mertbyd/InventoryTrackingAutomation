using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Stock;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Stock;

/// <summary>
/// Ürün stok uygulama servisi kontratı — 6 temel CRUD operasyonunu tanımlar.
/// </summary>
public interface IProductStockAppService : IApplicationService
{
    /// <summary> Id'ye göre tek stok kaydı getirir. </summary>
    Task<ProductStockDto> GetAsync(Guid id);

    /// <summary> Stok kayıtlarını sayfalı listeler. </summary>
    Task<PagedResultDto<ProductStockDto>> GetListAsync(PagedResultRequestDto input);

    /// <summary> Yeni stok kaydı oluşturur. </summary>
    Task<ProductStockDto> CreateAsync(CreateProductStockDto input);

    /// <summary> Birden fazla stok kaydını toplu oluşturur. </summary>
    Task<List<ProductStockDto>> CreateManyAsync(List<CreateProductStockDto> inputs);

    /// <summary> Stok kaydını günceller. </summary>
    Task<ProductStockDto> UpdateAsync(Guid id, UpdateProductStockDto input);

    /// <summary> Stok kaydını soft delete ile siler. </summary>
    Task DeleteAsync(Guid id);
}
