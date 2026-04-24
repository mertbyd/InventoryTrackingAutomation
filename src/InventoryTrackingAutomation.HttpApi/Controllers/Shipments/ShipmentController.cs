using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InventoryTrackingAutomation.Dtos.Shipments;
using InventoryTrackingAutomation.Services.Shipments;

namespace InventoryTrackingAutomation.Controllers.Shipments;

/// <summary>
/// Sevkiyat CRUD endpoint'leri.
/// </summary>
[Route("api/shipments")]
//[Authorize]
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
    public async Task<ShipmentDto> Get(Guid id) => await _appService.GetAsync(id);

    /// <summary> TÃ¼m sevkiyatlarÄ± listeler. </summary>
    [HttpGet]
    public async Task<Volo.Abp.Application.Dtos.PagedResultDto<ShipmentDto>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input) => await _appService.GetListAsync(input);

    /// <summary> Yeni sevkiyat oluÅŸturur. </summary>
    [HttpPost]
    public async Task<ShipmentDto> Create([FromBody] CreateShipmentDto input) => await _appService.CreateAsync(input);

    /// <summary> Birden fazla sevkiyatÄ± toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    public async Task<List<ShipmentDto>> CreateMany([FromBody] List<CreateShipmentDto> inputs) => await _appService.CreateManyAsync(inputs);

    /// <summary> SevkiyatÄ± gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    public async Task<ShipmentDto> Update(Guid id, [FromBody] UpdateShipmentDto input) => await _appService.UpdateAsync(id, input);

    /// <summary> SevkiyatÄ± soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    public async Task Delete(Guid id) => await _appService.DeleteAsync(id);
}

