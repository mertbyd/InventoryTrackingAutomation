using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Services.Masters;

namespace InventoryTrackingAutomation.Controllers.Masters;

/// <summary>
/// AraÃ§ CRUD endpoint'leri.
/// </summary>
[Route("api/vehicles")]
//[Authorize]
[ApiExplorerSettings(GroupName = "Masters")]
[Tags("Vehicles")]
public class VehicleController : InventoryTrackingAutomationController
{
    private readonly IVehicleAppService _appService;

    public VehicleController(IVehicleAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Id'ye gÃ¶re tek araÃ§ getirir. </summary>
    [HttpGet("{id}")]
    public async Task<VehicleDto> Get(Guid id) => await _appService.GetAsync(id);

    /// <summary> TÃ¼m araÃ§larÄ± listeler. </summary>
    [HttpGet]
    public async Task<Volo.Abp.Application.Dtos.PagedResultDto<VehicleDto>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input) => await _appService.GetListAsync(input);

    /// <summary> Yeni araÃ§ oluÅŸturur. </summary>
    [HttpPost]
    public async Task<VehicleDto> Create([FromBody] CreateVehicleDto input) => await _appService.CreateAsync(input);

    /// <summary> Birden fazla aracÄ± toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    public async Task<List<VehicleDto>> CreateMany([FromBody] List<CreateVehicleDto> inputs) => await _appService.CreateManyAsync(inputs);

    /// <summary> AracÄ± gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    public async Task<VehicleDto> Update(Guid id, [FromBody] UpdateVehicleDto input) => await _appService.UpdateAsync(id, input);

    /// <summary> AracÄ± soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    public async Task Delete(Guid id) => await _appService.DeleteAsync(id);
}

