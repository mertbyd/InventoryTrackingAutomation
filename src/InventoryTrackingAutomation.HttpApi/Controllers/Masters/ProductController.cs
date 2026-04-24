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
/// ÃœrÃ¼n CRUD endpoint'leri.
/// </summary>
[Route("api/products")]
//[Authorize]
[ApiExplorerSettings(GroupName = "Masters")]
[Tags("Products")]
public class ProductController : InventoryTrackingAutomationController
{
    private readonly IProductAppService _appService;

    public ProductController(IProductAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Id'ye gÃ¶re tek Ã¼rÃ¼n getirir. </summary>
    [HttpGet("{id}")]
    public async Task<ProductDto> Get(Guid id) => await _appService.GetAsync(id);

    /// <summary> TÃ¼m Ã¼rÃ¼nleri listeler. </summary>
    [HttpGet]
    public async Task<Volo.Abp.Application.Dtos.PagedResultDto<ProductDto>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input) => await _appService.GetListAsync(input);

    /// <summary> Yeni Ã¼rÃ¼n oluÅŸturur. </summary>
    [HttpPost]
    public async Task<ProductDto> Create([FromBody] CreateProductDto input) => await _appService.CreateAsync(input);

    /// <summary> Birden fazla Ã¼rÃ¼nÃ¼ toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    public async Task<List<ProductDto>> CreateMany([FromBody] List<CreateProductDto> inputs) => await _appService.CreateManyAsync(inputs);

    /// <summary> ÃœrÃ¼nÃ¼ gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    public async Task<ProductDto> Update(Guid id, [FromBody] UpdateProductDto input) => await _appService.UpdateAsync(id, input);

    /// <summary> ÃœrÃ¼nÃ¼ soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    public async Task Delete(Guid id) => await _appService.DeleteAsync(id);
}

