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
/// Lokasyon CRUD endpoint'leri.
/// </summary>
[Route("api/sites")]
//[Authorize]
[ApiExplorerSettings(GroupName = "Masters")]
[Tags("Sites")]
public class SiteController : InventoryTrackingAutomationController
{
    private readonly ISiteAppService _appService;

    public SiteController(ISiteAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Id'ye gÃ¶re tek lokasyon getirir. </summary>
    [HttpGet("{id}")]
    public async Task<SiteDto> Get(Guid id) => await _appService.GetAsync(id);

    /// <summary> TÃ¼m lokasyonlarÄ± listeler. </summary>
    [HttpGet]
    public async Task<Volo.Abp.Application.Dtos.PagedResultDto<SiteDto>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input) => await _appService.GetListAsync(input);

    /// <summary> Yeni lokasyon oluÅŸturur. </summary>
    [HttpPost]
    public async Task<SiteDto> Create([FromBody] CreateSiteDto input) => await _appService.CreateAsync(input);

    /// <summary> Birden fazla lokasyonu toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    public async Task<List<SiteDto>> CreateMany([FromBody] List<CreateSiteDto> inputs) => await _appService.CreateManyAsync(inputs);

    /// <summary> Lokasyonu gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    public async Task<SiteDto> Update(Guid id, [FromBody] UpdateSiteDto input) => await _appService.UpdateAsync(id, input);

    /// <summary> Lokasyonu soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    public async Task Delete(Guid id) => await _appService.DeleteAsync(id);
}

