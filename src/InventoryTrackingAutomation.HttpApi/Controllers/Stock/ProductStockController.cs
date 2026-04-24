using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InventoryTrackingAutomation.Dtos.Stock;
using InventoryTrackingAutomation.Services.Stock;

namespace InventoryTrackingAutomation.Controllers.Stock;

/// <summary>
/// ÃœrÃ¼n stok CRUD endpoint'leri.
/// </summary>
[Route("api/product-stocks")]
//[Authorize]
[ApiExplorerSettings(GroupName = "Stock")]
[Tags("ProductStocks")]
public class ProductStockController : InventoryTrackingAutomationController
{
    private readonly IProductStockAppService _appService;

    public ProductStockController(IProductStockAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Id'ye gÃ¶re tek stok kaydÄ± getirir. </summary>
    [HttpGet("{id}")]
    public async Task<ProductStockDto> Get(Guid id) => await _appService.GetAsync(id);

    /// <summary> TÃ¼m stok kayÄ±tlarÄ±nÄ± listeler. </summary>
    [HttpGet]
    public async Task<Volo.Abp.Application.Dtos.PagedResultDto<ProductStockDto>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input) => await _appService.GetListAsync(input);

    /// <summary> Yeni stok kaydÄ± oluÅŸturur. </summary>
    [HttpPost]
    public async Task<ProductStockDto> Create([FromBody] CreateProductStockDto input) => await _appService.CreateAsync(input);

    /// <summary> Birden fazla stok kaydÄ±nÄ± toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    public async Task<List<ProductStockDto>> CreateMany([FromBody] List<CreateProductStockDto> inputs) => await _appService.CreateManyAsync(inputs);

    /// <summary> Stok kaydÄ±nÄ± gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    public async Task<ProductStockDto> Update(Guid id, [FromBody] UpdateProductStockDto input) => await _appService.UpdateAsync(id, input);

    /// <summary> Stok kaydÄ±nÄ± soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    public async Task Delete(Guid id) => await _appService.DeleteAsync(id);
}

