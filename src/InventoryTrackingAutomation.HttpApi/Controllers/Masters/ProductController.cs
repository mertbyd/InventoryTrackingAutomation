using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Dtos.Inventory;
using InventoryTrackingAutomation.Services.Masters;
using InventoryTrackingAutomation.Permissions;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Controllers.Masters;

/// <summary>
/// Ürün CRUD endpoint'leri.
/// </summary>
[Route("api/products")]
[ApiExplorerSettings(GroupName = "Masters")]
public class ProductController : InventoryTrackingAutomationController
{
    public ProductController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IProductAppService _appService => LazyGetRequiredService<IProductAppService>();

    /// <summary>
    /// Ürün kaydını Id ile getirir.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <remarks>
    /// Response {
    ///   Code           (string)       → Ürün kodu
    ///   Name           (string)       → Ürün adı
    ///   CategoryId     (Guid?)        → Bağlı kategori Id'si
    ///   UnitTypeId     (Guid)  → Olcu birimi lookup Id'si
    ///   IsActive       (bool)         → Aktif mi
    ///   IsSerializable (bool)         → Seri numaralı mı
    /// }
    /// </remarks>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
    public async Task<Result<ProductDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary>
    /// Ürünün lokasyon bazlı stok özetini getirir.
    /// </summary>
    /// <param name="id">Ürün Id'si.</param>
    /// <remarks>
    /// Response {
    ///   ProductId           (Guid) → Ürün Id'si
    ///   TotalQuantity       (int)  → Toplam stok miktarı
    ///   WarehouseQuantity   (int)  → Depolardaki toplam miktar
    ///   VehicleQuantity     (int)  → Araçlardaki toplam miktar
    ///   ActiveTaskQuantity  (int)  → Aktif görevlerdeki toplam miktar
    /// }
    /// </remarks>
    [HttpGet("{id}/stock-summary")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<ProductStockSummaryDto>> GetStockSummary(Guid id)
    {
        var result = await _appService.GetStockSummaryAsync(id);
        return result;
    }

    /// <summary>
    /// Ürün kayıtlarını sayfalı liste olarak getirir.
    /// </summary>
    /// <param name="input">Sayfalama parametreleri.</param>
    /// <remarks>
    /// Response {
    ///   Code           (string)       → Ürün kodu
    ///   Name           (string)       → Ürün adı
    ///   CategoryId     (Guid?)        → Bağlı kategori Id'si
    ///   UnitTypeId     (Guid)  → Olcu birimi lookup Id'si
    ///   IsActive       (bool)         → Aktif mi
    ///   IsSerializable (bool)         → Seri numaralı mı
    /// }
    /// </remarks>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
    public async Task<Result<Volo.Abp.Application.Dtos.PagedResultDto<ProductDto>>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary>
    /// Yeni ürün kaydı oluşturur.
    /// </summary>
    /// <param name="input">Ürün bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   Code           (string)       → Ürün kodu
    ///   Name           (string)       → Ürün adı
    ///   CategoryId     (Guid?)        → Bağlı kategori Id'si
    ///   UnitTypeId     (Guid)  → Olcu birimi lookup Id'si
    ///   IsActive       (bool)         → Aktif mi
    ///   IsSerializable (bool)         → Seri numaralı mı
    /// }
    /// Response {
    ///   Code           (string)       → Ürün kodu
    ///   Name           (string)       → Ürün adı
    ///   CategoryId     (Guid?)        → Bağlı kategori Id'si
    ///   UnitTypeId     (Guid)  → Olcu birimi lookup Id'si
    ///   IsActive       (bool)         → Aktif mi
    ///   IsSerializable (bool)         → Seri numaralı mı
    /// }
    /// </remarks>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<ProductDto>> Create([FromBody] CreateProductDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary>
    /// Birden fazla ürün kaydını toplu oluşturur.
    /// </summary>
    /// <param name="inputs">Ürün bilgileri listesi.</param>
    /// <remarks>
    /// Request {
    ///   Code           (string)       → Ürün kodu
    ///   Name           (string)       → Ürün adı
    ///   CategoryId     (Guid?)        → Bağlı kategori Id'si
    ///   UnitTypeId     (Guid)  → Olcu birimi lookup Id'si
    ///   IsActive       (bool)         → Aktif mi
    ///   IsSerializable (bool)         → Seri numaralı mı
    /// }
    /// Response {
    ///   Code           (string)       → Ürün kodu
    ///   Name           (string)       → Ürün adı
    ///   CategoryId     (Guid?)        → Bağlı kategori Id'si
    ///   UnitTypeId     (Guid)  → Olcu birimi lookup Id'si
    ///   IsActive       (bool)         → Aktif mi
    ///   IsSerializable (bool)         → Seri numaralı mı
    /// }
    /// </remarks>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<List<ProductDto>>> CreateMany([FromBody] List<CreateProductDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary>
    /// Ürün kaydını günceller.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <param name="input">Güncel ürün bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   Code           (string)       → Ürün kodu
    ///   Name           (string)       → Ürün adı
    ///   CategoryId     (Guid?)        → Bağlı kategori Id'si
    ///   UnitTypeId     (Guid)  → Olcu birimi lookup Id'si
    ///   IsActive       (bool)         → Aktif mi
    ///   IsSerializable (bool)         → Seri numaralı mı
    /// }
    /// Response {
    ///   Code           (string)       → Ürün kodu
    ///   Name           (string)       → Ürün adı
    ///   CategoryId     (Guid?)        → Bağlı kategori Id'si
    ///   UnitTypeId     (Guid)  → Olcu birimi lookup Id'si
    ///   IsActive       (bool)         → Aktif mi
    ///   IsSerializable (bool)         → Seri numaralı mı
    /// }
    /// </remarks>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<ProductDto>> Update(Guid id, [FromBody] UpdateProductDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary>
    /// Urun kaydini silmek yerine pasife alir.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}
