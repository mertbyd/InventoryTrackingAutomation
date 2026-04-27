using InventoryTrackingAutomation.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Stock;
using InventoryTrackingAutomation.Services.Stock;

namespace InventoryTrackingAutomation.Controllers.Stock;

/// <summary>
/// ÃœrÃ¼n stok CRUD endpoint'leri.
/// </summary>
[Route("api/product-stocks")]
[ApiExplorerSettings(GroupName = "Stock")]
[Tags("ProductStocks")]
public class ProductStockController : InventoryTrackingAutomationController
{
    private readonly IProductStockAppService _appService;

    public ProductStockController(IProductStockAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Id'ye gÃ¶re tek stok kaydÄ± getirir. </summary>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<ProductStockDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary> TÃ¼m stok kayÄ±tlarÄ±nÄ± listeler. </summary>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<Volo.Abp.Application.Dtos.PagedResultDto<ProductStockDto>>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary> Yeni stok kaydÄ± oluÅŸturur. </summary>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<ProductStockDto>> Create([FromBody] CreateProductStockDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary> Birden fazla stok kaydÄ±nÄ± toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<List<ProductStockDto>>> CreateMany([FromBody] List<CreateProductStockDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary> Stok kaydÄ±nÄ± gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<ProductStockDto>> Update(Guid id, [FromBody] UpdateProductStockDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary> Stok kaydÄ±nÄ± soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}


