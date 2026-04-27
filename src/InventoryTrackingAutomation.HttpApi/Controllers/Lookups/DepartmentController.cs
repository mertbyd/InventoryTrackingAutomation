using InventoryTrackingAutomation.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Services.Lookups;

namespace InventoryTrackingAutomation.Controllers.Lookups;

/// <summary>
/// Departman CRUD endpoint'leri.
/// </summary>
[Route("api/departments")]
[Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
[ApiExplorerSettings(GroupName = "Lookups")]
[Tags("Departments")]
public class DepartmentController : InventoryTrackingAutomationController
{
    private readonly IDepartmentAppService _appService;

    public DepartmentController(IDepartmentAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Id'ye gÃ¶re tek departman getirir. </summary>
    [HttpGet("{id}")]
    public async Task<Result<DepartmentDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary> TÃ¼m departmanlarÄ± listeler. </summary>
    [HttpGet]
    public async Task<Result<Volo.Abp.Application.Dtos.PagedResultDto<DepartmentDto>>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary> Yeni departman oluÅŸturur. </summary>
    [HttpPost]
    public async Task<Result<DepartmentDto>> Create([FromBody] CreateDepartmentDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary> Birden fazla departmanÄ± toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    public async Task<Result<List<DepartmentDto>>> CreateMany([FromBody] List<CreateDepartmentDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary> DepartmanÄ± gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    public async Task<Result<DepartmentDto>> Update(Guid id, [FromBody] UpdateDepartmentDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary> DepartmanÄ± soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}


