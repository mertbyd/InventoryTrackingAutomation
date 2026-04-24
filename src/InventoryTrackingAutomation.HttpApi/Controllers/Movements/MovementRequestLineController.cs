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
/// Hareket talebi satÄ±rÄ± CRUD endpoint'leri.
/// </summary>
[Route("api/movement-request-lines")]
//[Authorize]
[ApiExplorerSettings(GroupName = "Movements")]
[Tags("MovementRequestLines")]
public class MovementRequestLineController : InventoryTrackingAutomationController
{
    private readonly IMovementRequestLineAppService _appService;

    public MovementRequestLineController(IMovementRequestLineAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Id'ye gÃ¶re tek hareket talebi satÄ±rÄ± getirir. </summary>
    [HttpGet("{id}")]
    public async Task<MovementRequestLineDto> Get(Guid id) => await _appService.GetAsync(id);

    /// <summary> TÃ¼m hareket talebi satÄ±rlarÄ±nÄ± listeler. </summary>
    [HttpGet]
    public async Task<Volo.Abp.Application.Dtos.PagedResultDto<MovementRequestLineDto>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input) => await _appService.GetListAsync(input);

    /// <summary> Yeni hareket talebi satÄ±rÄ± oluÅŸturur. </summary>
    [HttpPost]
    public async Task<MovementRequestLineDto> Create([FromBody] CreateMovementRequestLineDto input) => await _appService.CreateAsync(input);

    /// <summary> Birden fazla hareket talebi satÄ±rÄ±nÄ± toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    public async Task<List<MovementRequestLineDto>> CreateMany([FromBody] List<CreateMovementRequestLineDto> inputs) => await _appService.CreateManyAsync(inputs);

    /// <summary> Hareket talebi satÄ±rÄ±nÄ± gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    public async Task<MovementRequestLineDto> Update(Guid id, [FromBody] UpdateMovementRequestLineDto input) => await _appService.UpdateAsync(id, input);

    /// <summary> Hareket talebi satÄ±rÄ±nÄ± soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    public async Task Delete(Guid id) => await _appService.DeleteAsync(id);
}

