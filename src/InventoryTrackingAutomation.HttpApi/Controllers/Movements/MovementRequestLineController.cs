using InventoryTrackingAutomation.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Services.Movements;

namespace InventoryTrackingAutomation.Controllers.Movements;

/// <summary>
/// Hareket talebi satÄ±rÄ± CRUD endpoint'leri.
/// </summary>
[Route("api/movement-request-lines")]
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
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.View)]
    public async Task<Result<MovementRequestLineDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary> TÃ¼m hareket talebi satÄ±rlarÄ±nÄ± listeler. </summary>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.View)]
    public async Task<Result<Volo.Abp.Application.Dtos.PagedResultDto<MovementRequestLineDto>>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary> Yeni hareket talebi satÄ±rÄ± oluÅŸturur. </summary>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Create)]
    public async Task<Result<MovementRequestLineDto>> Create([FromBody] CreateMovementRequestLineDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary> Birden fazla hareket talebi satÄ±rÄ±nÄ± toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Create)]
    public async Task<Result<List<MovementRequestLineDto>>> CreateMany([FromBody] List<CreateMovementRequestLineDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary> Hareket talebi satÄ±rÄ±nÄ± gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Edit)]
    public async Task<Result<MovementRequestLineDto>> Update(Guid id, [FromBody] UpdateMovementRequestLineDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary> Hareket talebi satÄ±rÄ±nÄ± soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Delete)]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}


