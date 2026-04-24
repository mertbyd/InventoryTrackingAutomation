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
/// Stok hareketi CRUD endpoint'leri.
/// </summary>
[Route("api/stock-movements")]
//[Authorize]
[ApiExplorerSettings(GroupName = "Stock")]
[Tags("StockMovements")]
public class StockMovementController : InventoryTrackingAutomationController
{
    private readonly IStockMovementAppService _appService;

    public StockMovementController(IStockMovementAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Id'ye gÃ¶re tek stok hareketi getirir. </summary>
    [HttpGet("{id}")]
    public async Task<StockMovementDto> Get(Guid id) => await _appService.GetAsync(id);

    /// <summary> TÃ¼m stok hareketlerini listeler. </summary>
    [HttpGet]
    public async Task<Volo.Abp.Application.Dtos.PagedResultDto<StockMovementDto>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input) => await _appService.GetListAsync(input);

    /// <summary> Yeni stok hareketi oluÅŸturur. </summary>
    [HttpPost]
    public async Task<StockMovementDto> Create([FromBody] CreateStockMovementDto input) => await _appService.CreateAsync(input);

    /// <summary> Birden fazla stok hareketini toplu oluÅŸturur. </summary>
    [HttpPost("bulk")]
    public async Task<List<StockMovementDto>> CreateMany([FromBody] List<CreateStockMovementDto> inputs) => await _appService.CreateManyAsync(inputs);

    /// <summary> Stok hareketini gÃ¼nceller. </summary>
    [HttpPut("{id}")]
    public async Task<StockMovementDto> Update(Guid id, [FromBody] UpdateStockMovementDto input) => await _appService.UpdateAsync(id, input);

    /// <summary> Stok hareketini soft delete ile siler. </summary>
    [HttpDelete("{id}")]
    public async Task Delete(Guid id) => await _appService.DeleteAsync(id);
}

