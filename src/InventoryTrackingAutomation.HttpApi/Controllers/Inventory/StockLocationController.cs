using Asp.Versioning;
using InventoryTrackingAutomation.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Inventory;
using InventoryTrackingAutomation.Services.Inventory;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Controllers.Stock;

/// <summary>
/// Lokasyon bazli stok CRUD endpoint'leri.
/// </summary>
[Route("api/v{version:apiVersion}/stock-locations")]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Stock")]
[Tags("StockLocations")]
public class StockLocationController : InventoryTrackingAutomationController
{
    private readonly IStockLocationAppService _appService;

    public StockLocationController(IStockLocationAppService appService)
    {
        _appService = appService;
    }

    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<StockLocationDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<PagedResultDto<StockLocationDto>>> GetList([FromQuery] PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<StockLocationDto>> Create([FromBody] CreateStockLocationDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<List<StockLocationDto>>> CreateMany([FromBody] List<CreateStockLocationDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<StockLocationDto>> Update(Guid id, [FromBody] UpdateStockLocationDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}
