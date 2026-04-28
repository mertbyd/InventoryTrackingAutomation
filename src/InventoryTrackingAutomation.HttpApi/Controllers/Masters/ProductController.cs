using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Dtos.Inventory;
using InventoryTrackingAutomation.Services.Masters;
using InventoryTrackingAutomation.Permissions;

namespace InventoryTrackingAutomation.Controllers.Masters;

/// <summary>
/// Ürün CRUD endpoint'leri.
/// </summary>
[Route("api/products")]
[Route("api/v{version:apiVersion}/products")]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Masters")]
//işlevi: Product modülü için HTTP isteklerini karşılar.
//sistemdeki görevi: Dış dünya ile sistem arasındaki iletişimi sağlayan API uç noktasıdır.
public class ProductController : InventoryTrackingAutomationController
{
    private readonly IProductAppService _appService;

    public ProductController(IProductAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Id'ye göre tek ürün getirir. </summary>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<ProductDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary> Urunun depo/arac/gorev bazli stok ozetini getirir. </summary>
    [HttpGet("{id}/stock-summary")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<ProductStockSummaryDto>> GetStockSummary(Guid id)
    {
        var result = await _appService.GetStockSummaryAsync(id);
        return result;
    }

    /// <summary> Tüm ürünleri listeler. </summary>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<Volo.Abp.Application.Dtos.PagedResultDto<ProductDto>>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary> Yeni ürün oluşturur. </summary>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<ProductDto>> Create([FromBody] CreateProductDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary> Birden fazla ürünü toplu oluşturur. </summary>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<List<ProductDto>>> CreateMany([FromBody] List<CreateProductDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary> Ürünü günceller. </summary>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<ProductDto>> Update(Guid id, [FromBody] UpdateProductDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary> Ürünü soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}
