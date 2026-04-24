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
/// ÃœrÃ¼n kategorisi CRUD endpoint'leri.
/// </summary>
[Route("api/product-categories")]
//[Authorize]
[ApiExplorerSettings(GroupName = "Lookups")]
[Tags("ProductCategories")]
public class ProductCategoryController : InventoryTrackingAutomationController
{
    private readonly IProductCategoryAppService _appService;

    public ProductCategoryController(IProductCategoryAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Id'ye gÃ¶re tek Ã¼rÃ¼n kategorisi getirir. </summary>
    [HttpGet("{id}")]
    public async Task<ProductCategoryDto> Get(Guid id) => await _appService.GetAsync(id);

    /// <summary> TÃ¼m Ã¼rÃ¼n kategorilerini listeler. </summary>
    [HttpGet]
    public async Task<Volo.Abp.Application.Dtos.PagedResultDto<ProductCategoryDto>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input) => await _appService.GetListAsync(input);

    /// <summary> Yeni Ã¼rÃ¼n kategorisi oluÅŸturur. </summary>
    [HttpPost]
    public async Task<ProductCategoryDto> Create([FromBody] CreateProductCategoryDto input) => await _appService.CreateAsync(input);

    /// <summary> Birden fazla Ã¼rÃ¼n kategorisini toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    public async Task<List<ProductCategoryDto>> CreateMany([FromBody] List<CreateProductCategoryDto> inputs) => await _appService.CreateManyAsync(inputs);

    /// <summary> ÃœrÃ¼n kategorisini gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    public async Task<ProductCategoryDto> Update(Guid id, [FromBody] UpdateProductCategoryDto input) => await _appService.UpdateAsync(id, input);

    /// <summary> ÃœrÃ¼n kategorisini soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    public async Task Delete(Guid id) => await _appService.DeleteAsync(id);
}

