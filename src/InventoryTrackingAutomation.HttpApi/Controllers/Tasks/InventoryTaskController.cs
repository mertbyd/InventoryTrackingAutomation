using Asp.Versioning;
using InventoryTrackingAutomation.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Services.Tasks;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Controllers.Tasks;

/// <summary>
/// Envanter gorevi CRUD endpoint'leri.
/// </summary>
[Route("api/v{version:apiVersion}/tasks")]
[Route("api/v{version:apiVersion}/inventory-tasks")]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Tasks")]
[Tags("InventoryTasks")]
//işlevi: InventoryTask modülü için HTTP isteklerini karşılar.
//sistemdeki görevi: Dış dünya ile sistem arasındaki iletişimi sağlayan API uç noktasıdır.
public class InventoryTaskController : InventoryTrackingAutomationController
{
    private readonly IInventoryTaskAppService _appService;

    public InventoryTaskController(IInventoryTaskAppService appService)
    {
        _appService = appService;
    }

    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.View)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<InventoryTaskDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    [HttpGet("{id}/vehicles")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.View)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<List<TaskVehicleDto>>> GetVehicles(Guid id)
    {
        var result = await _appService.GetVehiclesAsync(id);
        return result;
    }

    [HttpGet("{id}/inventory")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<List<TaskInventoryDto>>> GetInventory(Guid id)
    {
        var result = await _appService.GetInventoryAsync(id);
        return result;
    }

    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.View)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<PagedResultDto<InventoryTaskDto>>> GetList([FromQuery] PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<InventoryTaskDto>> Create([FromBody] CreateInventoryTaskDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<List<InventoryTaskDto>>> CreateMany([FromBody] List<CreateInventoryTaskDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<InventoryTaskDto>> Update(Guid id, [FromBody] UpdateInventoryTaskDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    [HttpPost("{id}/complete")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.Complete)]
//iÅŸlevi: Ä°lgili HTTP isteÄŸini iÅŸler ve servis katmanÄ±na yÃ¶nlendirir.
//sistemdeki gÃ¶revi: Belirli bir API aksiyonunun giriÅŸ noktasÄ±nÄ± tanÄ±mlar.
    public async Task<Result<InventoryTaskDto>> Complete(Guid id)
    {
        var result = await _appService.CompleteAsync(id);
        return result;
    }

    [HttpPost("{id}/cancel")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.Complete)]
//iÅŸlevi: Ä°lgili HTTP isteÄŸini iÅŸler ve servis katmanÄ±na yÃ¶nlendirir.
//sistemdeki gÃ¶revi: Belirli bir API aksiyonunun giriÅŸ noktasÄ±nÄ± tanÄ±mlar.
    public async Task<Result<InventoryTaskDto>> Cancel(Guid id)
    {
        var result = await _appService.CancelAsync(id);
        return result;
    }

    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}
