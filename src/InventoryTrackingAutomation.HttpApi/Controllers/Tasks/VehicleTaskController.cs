using Asp.Versioning;
using InventoryTrackingAutomation.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Services.Tasks;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Controllers.Tasks;

/// <summary>
/// Arac-gorev atamasi CRUD endpoint'leri.
/// </summary>
[Route("api/v{version:apiVersion}/vehicle-tasks")]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Tasks")]
[Tags("VehicleTasks")]
//işlevi: VehicleTask modülü için HTTP isteklerini karşılar.
//sistemdeki görevi: Dış dünya ile sistem arasındaki iletişimi sağlayan API uç noktasıdır.
public class VehicleTaskController : InventoryTrackingAutomationController
{
    private readonly IVehicleTaskAppService _appService;

    public VehicleTaskController(IVehicleTaskAppService appService)
    {
        _appService = appService;
    }

    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTasks.View)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<VehicleTaskDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTasks.View)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<PagedResultDto<VehicleTaskDto>>> GetList([FromQuery] PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTasks.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<VehicleTaskDto>> Create([FromBody] CreateVehicleTaskDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTasks.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<List<VehicleTaskDto>>> CreateMany([FromBody] List<CreateVehicleTaskDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTasks.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<VehicleTaskDto>> Update(Guid id, [FromBody] UpdateVehicleTaskDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTasks.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}
