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
/// Sevkiyat satÄ±rÄ± CRUD endpoint'leri.
/// </summary>
[Route("api/shipment-lines")]
//[Authorize]
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
    public async Task<ShipmentLineDto> Get(Guid id) => await _appService.GetAsync(id);

    /// <summary> TÃ¼m sevkiyat satÄ±rlarÄ±nÄ± listeler. </summary>
    [HttpGet]
    public async Task<Volo.Abp.Application.Dtos.PagedResultDto<ShipmentLineDto>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input) => await _appService.GetListAsync(input);

    /// <summary> Yeni sevkiyat satÄ±rÄ± oluÅŸturur. </summary>
    [HttpPost]
    public async Task<ShipmentLineDto> Create([FromBody] CreateShipmentLineDto input) => await _appService.CreateAsync(input);

    /// <summary> Birden fazla sevkiyat satÄ±rÄ±nÄ± toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    public async Task<List<ShipmentLineDto>> CreateMany([FromBody] List<CreateShipmentLineDto> inputs) => await _appService.CreateManyAsync(inputs);

    /// <summary> Sevkiyat satÄ±rÄ±nÄ± gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    public async Task<ShipmentLineDto> Update(Guid id, [FromBody] UpdateShipmentLineDto input) => await _appService.UpdateAsync(id, input);

    /// <summary> Sevkiyat satÄ±rÄ±nÄ± soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    public async Task Delete(Guid id) => await _appService.DeleteAsync(id);
}

