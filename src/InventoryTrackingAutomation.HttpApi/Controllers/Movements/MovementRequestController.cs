using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Services.Movements;

using InventoryTrackingAutomation.Permissions;

namespace InventoryTrackingAutomation.Controllers.Movements;

/// <summary>
/// Hareket talebi CRUD endpoint'leri.
/// </summary>
[Route("api/movement-requests")]
[ApiExplorerSettings(GroupName = "Movements")]
[Tags("MovementRequests")]
//işlevi: MovementRequest modülü için HTTP isteklerini karşılar.
//sistemdeki görevi: Dış dünya ile sistem arasındaki iletişimi sağlayan API uç noktasıdır.
public class MovementRequestController : InventoryTrackingAutomationController
{
    private readonly IMovementRequestAppService _appService;

    public MovementRequestController(IMovementRequestAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Id'ye gÃ¶re tek hareket talebi getirir. </summary>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.View)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<MovementRequestDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary> TÃ¼m hareket taleplerini listeler. </summary>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.View)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<Volo.Abp.Application.Dtos.PagedResultDto<MovementRequestDto>>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary> Yeni hareket talebi oluÅŸturur. </summary>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Create)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<MovementRequestDto>> Create([FromBody] CreateMovementRequestDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary> Talebi ve satırlarını tek atomik istekle oluşturur. </summary>
    [HttpPost("with-lines")]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Create)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<MovementRequestDto>> CreateWithLines([FromBody] CreateMovementRequestWithLinesDto input)
    {
        var result = await _appService.CreateWithLinesAsync(input);
        return result;
    }

    /// <summary> Birden fazla hareket talebini toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Create)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<List<MovementRequestDto>>> CreateMany([FromBody] List<CreateMovementRequestDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary> Hareket talebini gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Edit)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<MovementRequestDto>> Update(Guid id, [FromBody] UpdateMovementRequestDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary> Hareket talebini soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Delete)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}
