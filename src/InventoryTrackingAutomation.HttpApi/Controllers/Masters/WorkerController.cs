using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Services.Masters;

namespace InventoryTrackingAutomation.Controllers.Masters;

/// <summary>
/// Ã‡alÄ±ÅŸan CRUD endpoint'leri.
/// </summary>
[Route("api/workers")]
//[Authorize]
[ApiExplorerSettings(GroupName = "Masters")]
[Tags("Workers")]
public class WorkerController : InventoryTrackingAutomationController
{
    private readonly IWorkerAppService _appService;

    public WorkerController(IWorkerAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Id'ye gÃ¶re tek Ã§alÄ±ÅŸan getirir. </summary>
    [HttpGet("{id}")]
    public async Task<WorkerDto> Get(Guid id) => await _appService.GetAsync(id);

    /// <summary> TÃ¼m Ã§alÄ±ÅŸanlarÄ± listeler. </summary>
    [HttpGet]
    public async Task<Volo.Abp.Application.Dtos.PagedResultDto<WorkerDto>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input) => await _appService.GetListAsync(input);

    /// <summary> Yeni Ã§alÄ±ÅŸan oluÅŸturur. </summary>
    [HttpPost]
    public async Task<WorkerDto> Create([FromBody] CreateWorkerDto input) => await _appService.CreateAsync(input);

    /// <summary> Birden fazla Ã§alÄ±ÅŸanÄ± toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    public async Task<List<WorkerDto>> CreateMany([FromBody] List<CreateWorkerDto> inputs) => await _appService.CreateManyAsync(inputs);

    /// <summary> Ã‡alÄ±ÅŸanÄ± gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    public async Task<WorkerDto> Update(Guid id, [FromBody] UpdateWorkerDto input) => await _appService.UpdateAsync(id, input);

    /// <summary> Ã‡alÄ±ÅŸanÄ± soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    public async Task Delete(Guid id) => await _appService.DeleteAsync(id);
}

