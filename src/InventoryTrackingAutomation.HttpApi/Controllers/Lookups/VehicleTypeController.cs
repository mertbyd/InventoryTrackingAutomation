using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Permissions;
using InventoryTrackingAutomation.Services.Lookups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SystemStandards.Results;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Controllers.Lookups;

/// <summary>
/// Arac tipi lookup CRUD endpoint'leri.
/// </summary>
[Route("api/vehicle-types")]
[ApiExplorerSettings(GroupName = "Lookups")]
[Tags("VehicleTypes")]
public class VehicleTypeController : InventoryTrackingAutomationController
{
    public VehicleTypeController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IVehicleTypeAppService _appService => LazyGetRequiredService<IVehicleTypeAppService>();

    /// <summary>
    /// Arac tipi kaydini Id ile getirir.
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
    public async Task<Result<VehicleTypeDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary>
    /// Arac tipi kayitlarini sayfali liste olarak getirir.
    /// </summary>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
    public async Task<Result<PagedResultDto<VehicleTypeDto>>> GetList([FromQuery] PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary>
    /// Yeni arac tipi kaydi olusturur.
    /// </summary>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<VehicleTypeDto>> Create([FromBody] CreateVehicleTypeDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary>
    /// Birden fazla arac tipi kaydini toplu olusturur.
    /// </summary>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<List<VehicleTypeDto>>> CreateMany([FromBody] List<CreateVehicleTypeDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary>
    /// Arac tipi kaydini gunceller.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<VehicleTypeDto>> Update(Guid id, [FromBody] UpdateVehicleTypeDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary>
    /// Arac tipi kaydini siler.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}
