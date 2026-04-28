using InventoryTrackingAutomation.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Services.Masters;

namespace InventoryTrackingAutomation.Controllers.Masters;

/// <summary>
/// Ã‡alÄ±ÅŸan CRUD endpoint'leri.
/// </summary>
[Route("api/workers")]
[ApiExplorerSettings(GroupName = "Masters")]
[Tags("Workers")]
//işlevi: Worker modülü için HTTP isteklerini karşılar.
//sistemdeki görevi: Dış dünya ile sistem arasındaki iletişimi sağlayan API uç noktasıdır.
public class WorkerController : InventoryTrackingAutomationController
{
    private readonly IWorkerAppService _appService;

    public WorkerController(IWorkerAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Id'ye gÃ¶re tek Ã§alÄ±ÅŸan getirir. </summary>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<WorkerDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary> TÃ¼m Ã§alÄ±ÅŸanlarÄ± listeler. </summary>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<Volo.Abp.Application.Dtos.PagedResultDto<WorkerDto>>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary> Yeni Ã§alÄ±ÅŸan oluÅŸturur. </summary>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<WorkerDto>> Create([FromBody] CreateWorkerDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary> Birden fazla Ã§alÄ±ÅŸanÄ± toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<List<WorkerDto>>> CreateMany([FromBody] List<CreateWorkerDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary> Ã‡alÄ±ÅŸanÄ± gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<WorkerDto>> Update(Guid id, [FromBody] UpdateWorkerDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary> Ã‡alÄ±ÅŸanÄ± soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}


