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
/// Envanter hareketleri CRUD endpoint'leri.
/// </summary>
[Route("api/v{version:apiVersion}/inventory-transactions")]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Stock")]
[Tags("InventoryTransactions")]
public class InventoryTransactionController : InventoryTrackingAutomationController
{
    private readonly IInventoryTransactionAppService _appService;

    public InventoryTransactionController(IInventoryTransactionAppService appService)
    {
        _appService = appService;
    }

    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<InventoryTransactionDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<PagedResultDto<InventoryTransactionDto>>> GetList([FromQuery] PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<InventoryTransactionDto>> Create([FromBody] CreateInventoryTransactionDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<List<InventoryTransactionDto>>> CreateMany([FromBody] List<CreateInventoryTransactionDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<InventoryTransactionDto>> Update(Guid id, [FromBody] UpdateInventoryTransactionDto input)
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
