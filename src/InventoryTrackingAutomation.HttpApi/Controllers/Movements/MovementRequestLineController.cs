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
//işlevi: MovementRequestLine modülü için HTTP isteklerini karşılar.
//sistemdeki görevi: Dış dünya ile sistem arasındaki iletişimi sağlayan API uç noktasıdır.
public class MovementRequestLineController : InventoryTrackingAutomationController
{
    private readonly IMovementRequestLineAppService _appService;

    public MovementRequestLineController(IMovementRequestLineAppService appService)
    {
        _appService = appService;
    }

    /// Hareket talebi satır verisini getirmek için kullanılır.
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.View)]
    public async Task<Result<MovementRequestLineDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// Hareket talebi satır listesini getirmek için kullanılır.
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.View)]
    public async Task<Result<Volo.Abp.Application.Dtos.PagedResultDto<MovementRequestLineDto>>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// Yeni bir hareket talebi satırı oluşturmak için kullanılır.
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Create)]
    public async Task<Result<MovementRequestLineDto>> Create([FromBody] CreateMovementRequestLineDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// Birden fazla hareket talebi satırını toplu olarak oluşturmak için kullanılır.
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Create)]
    public async Task<Result<List<MovementRequestLineDto>>> CreateMany([FromBody] List<CreateMovementRequestLineDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// Hareket talebi satırını güncellemek için kullanılır.
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Edit)]
    public async Task<Result<MovementRequestLineDto>> Update(Guid id, [FromBody] UpdateMovementRequestLineDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// Hareket talebi satırını silmek için kullanılır.
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Delete)]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}


