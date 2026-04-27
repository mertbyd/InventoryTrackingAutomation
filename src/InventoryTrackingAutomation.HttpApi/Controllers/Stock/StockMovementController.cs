using InventoryTrackingAutomation.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Stock;
using InventoryTrackingAutomation.Services.Stock;

namespace InventoryTrackingAutomation.Controllers.Stock;

/// <summary>
/// Stok hareketi CRUD endpoint'leri.
/// </summary>
[Route("api/stock-movements")]
[Authorize]
[ApiExplorerSettings(GroupName = "Stock")]
[Tags("StockMovements")]
public class StockMovementController : InventoryTrackingAutomationController
{
    private readonly IStockMovementAppService _appService;

    public StockMovementController(IStockMovementAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Id'ye gÃ¶re tek stok hareketi getirir. </summary>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<StockMovementDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary> TÃ¼m stok hareketlerini listeler. </summary>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<Volo.Abp.Application.Dtos.PagedResultDto<StockMovementDto>>> GetList(
        [FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary> Yeni stok hareketi oluÅŸturur. </summary>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<StockMovementDto>> Create([FromBody] CreateStockMovementDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary> Birden fazla stok hareketini toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<List<StockMovementDto>>> CreateMany([FromBody] List<CreateStockMovementDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary> Stok hareketini gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<StockMovementDto>> Update(Guid id, [FromBody] UpdateStockMovementDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary> Stok hareketini soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}

