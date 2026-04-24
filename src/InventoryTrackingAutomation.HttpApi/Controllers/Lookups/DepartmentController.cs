using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Services.Lookups;

namespace InventoryTrackingAutomation.Controllers.Lookups;

/// <summary>
/// Departman CRUD endpoint'leri.
/// </summary>
[Route("api/departments")]
//[Authorize]
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
    public async Task<DepartmentDto> Get(Guid id) => await _appService.GetAsync(id);

    /// <summary> TÃ¼m departmanlarÄ± listeler. </summary>
    [HttpGet]
    public async Task<Volo.Abp.Application.Dtos.PagedResultDto<DepartmentDto>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input) => await _appService.GetListAsync(input);

    /// <summary> Yeni departman oluÅŸturur. </summary>
    [HttpPost]
    public async Task<DepartmentDto> Create([FromBody] CreateDepartmentDto input) => await _appService.CreateAsync(input);

    /// <summary> Birden fazla departmanÄ± toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    public async Task<List<DepartmentDto>> CreateMany([FromBody] List<CreateDepartmentDto> inputs) => await _appService.CreateManyAsync(inputs);

    /// <summary> DepartmanÄ± gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    public async Task<DepartmentDto> Update(Guid id, [FromBody] UpdateDepartmentDto input) => await _appService.UpdateAsync(id, input);

    /// <summary> DepartmanÄ± soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    public async Task Delete(Guid id) => await _appService.DeleteAsync(id);
}

