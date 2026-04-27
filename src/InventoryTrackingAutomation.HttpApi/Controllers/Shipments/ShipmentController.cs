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
/// Sevkiyat CRUD endpoint'leri.
/// </summary>
[Route("api/shipments")]
[ApiExplorerSettings(GroupName = "Shipments")]
[Tags("Shipments")]
public class ShipmentController : InventoryTrackingAutomationController
{
    private readonly IShipmentAppService _appService;

    public ShipmentController(IShipmentAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Id'ye gÃ¶re tek sevkiyat getirir. </summary>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<ShipmentDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary> TÃ¼m sevkiyatlarÄ± listeler. </summary>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<Volo.Abp.Application.Dtos.PagedResultDto<ShipmentDto>>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary> Yeni sevkiyat oluÅŸturur. </summary>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<ShipmentDto>> Create([FromBody] CreateShipmentDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary> Birden fazla sevkiyatÄ± toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<List<ShipmentDto>>> CreateMany([FromBody] List<CreateShipmentDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary> SevkiyatÄ± gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<ShipmentDto>> Update(Guid id, [FromBody] UpdateShipmentDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary> SevkiyatÄ± soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}


