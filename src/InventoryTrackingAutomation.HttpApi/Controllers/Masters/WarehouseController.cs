using Asp.Versioning;
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
/// Depo CRUD endpoint'leri.
/// </summary>
[Route("api/v{version:apiVersion}/warehouses")]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Masters")]
[Tags("Warehouses")]
//işlevi: Warehouse modülü için HTTP isteklerini karşılar.
//sistemdeki görevi: Dış dünya ile sistem arasındaki iletişimi sağlayan API uç noktasıdır.
public class WarehouseController : InventoryTrackingAutomationController
{
    private readonly IWarehouseAppService _appService;

    public WarehouseController(IWarehouseAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Id'ye gÃ¶re tek lokasyon getirir. </summary>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<WarehouseDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary> TÃ¼m lokasyonlarÄ± listeler. </summary>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<Volo.Abp.Application.Dtos.PagedResultDto<WarehouseDto>>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary> Yeni lokasyon oluÅŸturur. </summary>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<WarehouseDto>> Create([FromBody] CreateWarehouseDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary> Birden fazla lokasyonu toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<List<WarehouseDto>>> CreateMany([FromBody] List<CreateWarehouseDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary> Lokasyonu gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
//işlevi: İlgili HTTP isteğini işler ve servis katmanına yönlendirir.
//sistemdeki görevi: Belirli bir API aksiyonunun giriş noktasını tanımlar.
    public async Task<Result<WarehouseDto>> Update(Guid id, [FromBody] UpdateWarehouseDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary> Lokasyonu soft delete ile siler. </summary>
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


