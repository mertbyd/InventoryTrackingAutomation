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

namespace InventoryTrackingAutomation.Controllers.Lookups;

/// <summary>
/// ÃœrÃ¼n kategorisi CRUD endpoint'leri.
/// </summary>
[Route("api/product-categories")]
[ApiExplorerSettings(GroupName = "Lookups")]
[Tags("ProductCategories")]
//işlevi: ProductCategory modülü için HTTP isteklerini karşılar.
//sistemdeki görevi: Dış dünya ile sistem arasındaki iletişimi sağlayan API uç noktasıdır.
public class ProductCategoryController : InventoryTrackingAutomationController
{
    private readonly IProductCategoryAppService _appService;

    public ProductCategoryController(IProductCategoryAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Id'ye gÃ¶re tek Ã¼rÃ¼n kategorisi getirir. </summary>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<ProductCategoryDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary> TÃ¼m Ã¼rÃ¼n kategorilerini listeler. </summary>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<Volo.Abp.Application.Dtos.PagedResultDto<ProductCategoryDto>>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary> Yeni Ã¼rÃ¼n kategorisi oluÅŸturur. </summary>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<ProductCategoryDto>> Create([FromBody] CreateProductCategoryDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary> Birden fazla Ã¼rÃ¼n kategorisini toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<List<ProductCategoryDto>>> CreateMany([FromBody] List<CreateProductCategoryDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary> ÃœrÃ¼n kategorisini gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<ProductCategoryDto>> Update(Guid id, [FromBody] UpdateProductCategoryDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary> ÃœrÃ¼n kategorisini soft delete ile siler. </summary>
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


