using InventoryTrackingAutomation.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Inventory;
using InventoryTrackingAutomation.Services.Inventory;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Controllers.Stock;

/// <summary>
/// Lokasyon bazlı stok CRUD endpoint'leri.
/// </summary>
[Route("api/stock-locations")]
[ApiExplorerSettings(GroupName = "Stock")]
[Tags("StockLocations")]
public class StockLocationController : InventoryTrackingAutomationController
{
    public StockLocationController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IStockLocationAppService _appService => LazyGetRequiredService<IStockLocationAppService>();

    /// <summary>
    /// Stok lokasyonu kaydını Id ile getirir.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <remarks>
    /// Response {
    ///   ProductId          (Guid)                   → Ürün Id'si
    ///   LocationType       (StockLocationTypeEnum)  → Lokasyon tipi
    ///   LocationId         (Guid)                   → Depo veya araç Id'si
    ///   Quantity           (int)                    → Stok miktarı
    ///   ReservedQuantity   (int)                    → Rezerve miktar
    /// }
    /// </remarks>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<StockLocationDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary>
    /// Stok lokasyonu kayıtlarını sayfalı liste olarak getirir.
    /// </summary>
    /// <param name="input">Sayfalama parametreleri.</param>
    /// <remarks>
    /// Response {
    ///   ProductId          (Guid)                   → Ürün Id'si
    ///   LocationType       (StockLocationTypeEnum)  → Lokasyon tipi
    ///   LocationId         (Guid)                   → Depo veya araç Id'si
    ///   Quantity           (int)                    → Stok miktarı
    ///   ReservedQuantity   (int)                    → Rezerve miktar
    /// }
    /// </remarks>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<PagedResultDto<StockLocationDto>>> GetList([FromQuery] PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary>
    /// Yeni stok lokasyonu kaydı oluşturur.
    /// </summary>
    /// <param name="input">Lokasyon bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   ProductId          (Guid)                   → Ürün Id'si
    ///   LocationType       (StockLocationTypeEnum)  → Lokasyon tipi
    ///   LocationId         (Guid)                   → Depo veya araç Id'si
    ///   Quantity           (int)                    → Stok miktarı
    ///   ReservedQuantity   (int)                    → Rezerve miktar
    /// }
    /// Response {
    ///   ProductId          (Guid)                   → Ürün Id'si
    ///   LocationType       (StockLocationTypeEnum)  → Lokasyon tipi
    ///   LocationId         (Guid)                   → Depo veya araç Id'si
    ///   Quantity           (int)                    → Stok miktarı
    ///   ReservedQuantity   (int)                    → Rezerve miktar
    /// }
    /// </remarks>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<StockLocationDto>> Create([FromBody] CreateStockLocationDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary>
    /// Birden fazla stok lokasyonu kaydını toplu oluşturur.
    /// </summary>
    /// <param name="inputs">Lokasyon bilgileri listesi.</param>
    /// <remarks>
    /// Request {
    ///   ProductId          (Guid)                   → Ürün Id'si
    ///   LocationType       (StockLocationTypeEnum)  → Lokasyon tipi
    ///   LocationId         (Guid)                   → Depo veya araç Id'si
    ///   Quantity           (int)                    → Stok miktarı
    ///   ReservedQuantity   (int)                    → Rezerve miktar
    /// }
    /// Response {
    ///   ProductId          (Guid)                   → Ürün Id'si
    ///   LocationType       (StockLocationTypeEnum)  → Lokasyon tipi
    ///   LocationId         (Guid)                   → Depo veya araç Id'si
    ///   Quantity           (int)                    → Stok miktarı
    ///   ReservedQuantity   (int)                    → Rezerve miktar
    /// }
    /// </remarks>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<List<StockLocationDto>>> CreateMany([FromBody] List<CreateStockLocationDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary>
    /// Stok lokasyonu kaydını günceller.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <param name="input">Güncel lokasyon bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   ProductId          (Guid)                   → Ürün Id'si
    ///   LocationType       (StockLocationTypeEnum)  → Lokasyon tipi
    ///   LocationId         (Guid)                   → Depo veya araç Id'si
    ///   Quantity           (int)                    → Stok miktarı
    ///   ReservedQuantity   (int)                    → Rezerve miktar
    /// }
    /// Response {
    ///   ProductId          (Guid)                   → Ürün Id'si
    ///   LocationType       (StockLocationTypeEnum)  → Lokasyon tipi
    ///   LocationId         (Guid)                   → Depo veya araç Id'si
    ///   Quantity           (int)                    → Stok miktarı
    ///   ReservedQuantity   (int)                    → Rezerve miktar
    /// }
    /// </remarks>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<StockLocationDto>> Update(Guid id, [FromBody] UpdateStockLocationDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary>
    /// Stok lokasyonu kaydını siler.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}
