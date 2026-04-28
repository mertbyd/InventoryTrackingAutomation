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
//işlevi: InventoryTransaction modülü için HTTP isteklerini karşılar.
//sistemdeki görevi: Dış dünya ile sistem arasındaki iletişimi sağlayan API uç noktasıdır.
public class InventoryTransactionController : InventoryTrackingAutomationController
{
    private readonly IInventoryTransactionAppService _appService;

    public InventoryTransactionController(IInventoryTransactionAppService appService)
    {
        _appService = appService;
    }

    /// Stok hareket verisini getirmek için kullanılır.
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<InventoryTransactionDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// Stok hareket listesini getirmek için kullanılır.
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<PagedResultDto<InventoryTransactionDto>>> GetList([FromQuery] PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// Yeni bir stok hareket kaydı oluşturmak için kullanılır.
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<InventoryTransactionDto>> Create([FromBody] CreateInventoryTransactionDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// Birden fazla stok hareket kaydını toplu olarak oluşturmak için kullanılır.
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<List<InventoryTransactionDto>>> CreateMany([FromBody] List<CreateInventoryTransactionDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// Mevcut bir stok hareket kaydını güncellemek için kullanılır.
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<InventoryTransactionDto>> Update(Guid id, [FromBody] UpdateInventoryTransactionDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// Stok hareket kaydını silmek için kullanılır.
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}
