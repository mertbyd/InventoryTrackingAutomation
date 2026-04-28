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
[Route("api/v1/tasks")]
[Route("api/v1/inventory-tasks")]
[ApiExplorerSettings(GroupName = "Tasks")]
[Tags("InventoryTasks")]
public class InventoryTaskController : InventoryTrackingAutomationController
{
    private readonly IInventoryTaskAppService _appService;

    public InventoryTaskController(IInventoryTaskAppService appService)
    {
        _appService = appService;
    }

    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.View)]
    public async Task<Result<InventoryTaskDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    [HttpGet("{id}/vehicles")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.View)]
    public async Task<Result<List<TaskVehicleDto>>> GetVehicles(Guid id)
    {
        var result = await _appService.GetVehiclesAsync(id);
        return result;
    }

    [HttpGet("{id}/inventory")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<List<TaskInventoryDto>>> GetInventory(Guid id)
    {
        var result = await _appService.GetInventoryAsync(id);
        return result;
    }

    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.View)]
    public async Task<Result<PagedResultDto<InventoryTaskDto>>> GetList([FromQuery] PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.Manage)]
    public async Task<Result<InventoryTaskDto>> Create([FromBody] CreateInventoryTaskDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.Manage)]
    public async Task<Result<List<InventoryTaskDto>>> CreateMany([FromBody] List<CreateInventoryTaskDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.Manage)]
    public async Task<Result<InventoryTaskDto>> Update(Guid id, [FromBody] UpdateInventoryTaskDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.Manage)]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}
