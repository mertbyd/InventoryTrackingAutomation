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
/// Olcu birimi lookup CRUD endpoint'leri.
/// </summary>
[Route("api/unit-types")]
[ApiExplorerSettings(GroupName = "Lookups")]
[Tags("UnitTypes")]
public class UnitTypeController : InventoryTrackingAutomationController
{
    public UnitTypeController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IUnitTypeAppService _appService => LazyGetRequiredService<IUnitTypeAppService>();

    /// <summary>
    /// Olcu birimi kaydini Id ile getirir.
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
    public async Task<Result<UnitTypeDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary>
    /// Olcu birimi kayitlarini sayfali liste olarak getirir.
    /// </summary>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
    public async Task<Result<PagedResultDto<UnitTypeDto>>> GetList([FromQuery] PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary>
    /// Yeni olcu birimi kaydi olusturur.
    /// </summary>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<UnitTypeDto>> Create([FromBody] CreateUnitTypeDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary>
    /// Birden fazla olcu birimi kaydini toplu olusturur.
    /// </summary>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<List<UnitTypeDto>>> CreateMany([FromBody] List<CreateUnitTypeDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary>
    /// Olcu birimi kaydini gunceller.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<UnitTypeDto>> Update(Guid id, [FromBody] UpdateUnitTypeDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary>
    /// Olcu birimi kaydini siler.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}
