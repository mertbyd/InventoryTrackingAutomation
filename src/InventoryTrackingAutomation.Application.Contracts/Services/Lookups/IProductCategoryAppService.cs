using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Lookups;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Lookups;

/// <summary>
/// ÃœrÃ¼n kategorisi uygulama servisi kontratÄ± â€” 6 temel CRUD operasyonunu tanÄ±mlar.
/// </summary>
public interface IProductCategoryAppService : IApplicationService
{
    /// <summary> Id'ye gÃ¶re tek Ã¼rÃ¼n kategorisi getirir. </summary>
    /// <summary>
    /// Id'ye göre tekil kaydý getirir.
    /// </summary>
    Task<ProductCategoryDto> GetAsync(Guid id);

    /// <summary> ÃœrÃ¼n kategorilerini sayfalÄ± listeler. </summary>
    /// <summary>
    /// Sayfalamalý listeleme yapar.
    /// </summary>
    Task<PagedResultDto<ProductCategoryDto>> GetListAsync(PagedResultRequestDto input);

    /// <summary> Yeni Ã¼rÃ¼n kategorisi oluÅŸturur. </summary>
    /// <summary>
    /// Yeni kayýt oluþturur.
    /// </summary>
    Task<ProductCategoryDto> CreateAsync(CreateProductCategoryDto input);

    /// <summary> Birden fazla Ã¼rÃ¼n kategorisini toplu oluÅŸturur. </summary>
    /// <summary>
    /// Toplu yeni kayýtlar oluþturur.
    /// </summary>
    Task<List<ProductCategoryDto>> CreateManyAsync(List<CreateProductCategoryDto> inputs);

    /// <summary> ÃœrÃ¼n kategorisini gÃ¼nceller. </summary>
    /// <summary>
    /// Ýlgili kaydý günceller.
    /// </summary>
    Task<ProductCategoryDto> UpdateAsync(Guid id, UpdateProductCategoryDto input);

    /// <summary> ÃœrÃ¼n kategorisini soft delete ile siler. </summary>
    /// <summary>
    /// Ýlgili kaydý siler.
    /// </summary>
    Task DeleteAsync(Guid id);
}

