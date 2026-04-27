using InventoryTrackingAutomation.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Shipments;
using InventoryTrackingAutomation.Services.Shipments;

namespace InventoryTrackingAutomation.Controllers.Shipments;

/// <summary>
/// Sevkiyat satÄ±rÄ± CRUD endpoint'leri.
/// </summary>
[Route("api/shipment-lines")]
[ApiExplorerSettings(GroupName = "Shipments")]
[Tags("ShipmentLines")]
public class ShipmentLineController : InventoryTrackingAutomationController
{
    private readonly IShipmentLineAppService _appService;

    public ShipmentLineController(IShipmentLineAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Id'ye gÃ¶re tek sevkiyat satÄ±rÄ± getirir. </summary>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<ShipmentLineDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary> TÃ¼m sevkiyat satÄ±rlarÄ±nÄ± listeler. </summary>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<Volo.Abp.Application.Dtos.PagedResultDto<ShipmentLineDto>>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary> Yeni sevkiyat satÄ±rÄ± oluÅŸturur. </summary>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<ShipmentLineDto>> Create([FromBody] CreateShipmentLineDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary> Birden fazla sevkiyat satÄ±rÄ±nÄ± toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<List<ShipmentLineDto>>> CreateMany([FromBody] List<CreateShipmentLineDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary> Sevkiyat satÄ±rÄ±nÄ± gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<ShipmentLineDto>> Update(Guid id, [FromBody] UpdateShipmentLineDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary> Sevkiyat satÄ±rÄ±nÄ± soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}


