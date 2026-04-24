using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Lookups;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Lookups;

/// <summary>
/// Ürün kategorisi uygulama servisi kontratı — 6 temel CRUD operasyonunu tanımlar.
/// </summary>
public interface IProductCategoryAppService : IApplicationService
{
    /// <summary> Id'ye göre tek ürün kategorisi getirir. </summary>
    Task<ProductCategoryDto> GetAsync(Guid id);

    /// <summary> Ürün kategorilerini sayfalı listeler. </summary>
    Task<PagedResultDto<ProductCategoryDto>> GetListAsync(PagedResultRequestDto input);

    /// <summary> Yeni ürün kategorisi oluşturur. </summary>
    Task<ProductCategoryDto> CreateAsync(CreateProductCategoryDto input);

    /// <summary> Birden fazla ürün kategorisini toplu oluşturur. </summary>
    Task<List<ProductCategoryDto>> CreateManyAsync(List<CreateProductCategoryDto> inputs);

    /// <summary> Ürün kategorisini günceller. </summary>
    Task<ProductCategoryDto> UpdateAsync(Guid id, UpdateProductCategoryDto input);

    /// <summary> Ürün kategorisini soft delete ile siler. </summary>
    Task DeleteAsync(Guid id);
}
