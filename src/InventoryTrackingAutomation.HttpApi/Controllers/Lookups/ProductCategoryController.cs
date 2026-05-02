using InventoryTrackingAutomation.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Services.Lookups;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Controllers.Lookups;

/// <summary>
/// Ürün kategorisi CRUD endpoint'leri.
/// </summary>
[Route("api/product-categories")]
[ApiExplorerSettings(GroupName = "Lookups")]
[Tags("ProductCategories")]
public class ProductCategoryController : InventoryTrackingAutomationController
{
    public ProductCategoryController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IProductCategoryAppService _appService => LazyGetRequiredService<IProductCategoryAppService>();

    /// <summary>
    /// Ürün kategorisi kaydını Id ile getirir.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <remarks>
    /// Response {
    ///   Code     (string) → Kategori kodu
    ///   Name     (string) → Kategori adı
    ///   ParentId (Guid?)  → Üst kategori Id'si
    /// }
    /// </remarks>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
    public async Task<Result<ProductCategoryDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary>
    /// Ürün kategorisi kayıtlarını sayfalı liste olarak getirir.
    /// </summary>
    /// <param name="input">Sayfalama parametreleri.</param>
    /// <remarks>
    /// Response {
    ///   Code     (string) → Kategori kodu
    ///   Name     (string) → Kategori adı
    ///   ParentId (Guid?)  → Üst kategori Id'si
    /// }
    /// </remarks>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
    public async Task<Result<Volo.Abp.Application.Dtos.PagedResultDto<ProductCategoryDto>>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary>
    /// Yeni ürün kategorisi kaydı oluşturur.
    /// </summary>
    /// <param name="input">Kategori bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   Code     (string) → Kategori kodu
    ///   Name     (string) → Kategori adı
    ///   ParentId (Guid?)  → Üst kategori Id'si
    /// }
    /// Response {
    ///   Code     (string) → Kategori kodu
    ///   Name     (string) → Kategori adı
    ///   ParentId (Guid?)  → Üst kategori Id'si
    /// }
    /// </remarks>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<ProductCategoryDto>> Create([FromBody] CreateProductCategoryDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary>
    /// Birden fazla ürün kategorisi kaydını toplu oluşturur.
    /// </summary>
    /// <param name="inputs">Kategori bilgileri listesi.</param>
    /// <remarks>
    /// Request {
    ///   Code     (string) → Kategori kodu
    ///   Name     (string) → Kategori adı
    ///   ParentId (Guid?)  → Üst kategori Id'si
    /// }
    /// Response {
    ///   Code     (string) → Kategori kodu
    ///   Name     (string) → Kategori adı
    ///   ParentId (Guid?)  → Üst kategori Id'si
    /// }
    /// </remarks>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<List<ProductCategoryDto>>> CreateMany([FromBody] List<CreateProductCategoryDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary>
    /// Ürün kategorisi kaydını günceller.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <param name="input">Güncel kategori bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   Code     (string) → Kategori kodu
    ///   Name     (string) → Kategori adı
    ///   ParentId (Guid?)  → Üst kategori Id'si
    /// }
    /// Response {
    ///   Code     (string) → Kategori kodu
    ///   Name     (string) → Kategori adı
    ///   ParentId (Guid?)  → Üst kategori Id'si
    /// }
    /// </remarks>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<ProductCategoryDto>> Update(Guid id, [FromBody] UpdateProductCategoryDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary>
    /// Ürün kategorisi kaydını siler.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}
