using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Services.Movements;

namespace InventoryTrackingAutomation.Controllers.Movements;

/// <summary>
/// Hareket talebi CRUD endpoint'leri.
/// </summary>
[Route("api/movement-requests")]
//[Authorize]
[ApiExplorerSettings(GroupName = "Movements")]
[Tags("MovementRequests")]
public class MovementRequestController : InventoryTrackingAutomationController
{
    private readonly IMovementRequestAppService _appService;

    public MovementRequestController(IMovementRequestAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Id'ye gÃ¶re tek hareket talebi getirir. </summary>
    [HttpGet("{id}")]
    public async Task<MovementRequestDto> Get(Guid id) => await _appService.GetAsync(id);

    /// <summary> TÃ¼m hareket taleplerini listeler. </summary>
    [HttpGet]
    public async Task<Volo.Abp.Application.Dtos.PagedResultDto<MovementRequestDto>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input) => await _appService.GetListAsync(input);

    /// <summary> Yeni hareket talebi oluÅŸturur. </summary>
    [HttpPost]
    public async Task<MovementRequestDto> Create([FromBody] CreateMovementRequestDto input) => await _appService.CreateAsync(input);

    /// <summary> Birden fazla hareket talebini toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    public async Task<List<MovementRequestDto>> CreateMany([FromBody] List<CreateMovementRequestDto> inputs) => await _appService.CreateManyAsync(inputs);

    /// <summary> Hareket talebini gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    public async Task<MovementRequestDto> Update(Guid id, [FromBody] UpdateMovementRequestDto input) => await _appService.UpdateAsync(id, input);

    /// <summary> Hareket talebini soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    public async Task Delete(Guid id) => await _appService.DeleteAsync(id);
}

