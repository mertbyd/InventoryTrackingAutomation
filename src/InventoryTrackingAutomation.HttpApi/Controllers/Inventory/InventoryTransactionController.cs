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
/// Envanter hareketleri append-only ledger endpoint'leri.
/// </summary>
[Route("api/inventory-transactions")]
[ApiExplorerSettings(GroupName = "Stock")]
[Tags("InventoryTransactions")]
public class InventoryTransactionController : InventoryTrackingAutomationController
{
    public InventoryTransactionController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IInventoryTransactionAppService _appService => LazyGetRequiredService<IInventoryTransactionAppService>();

    /// <summary>
    /// Envanter hareketi kaydını Id ile getirir.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <remarks>
    /// Response {
    ///   ProductId                 (Guid)                         → Ürün Id'si
    ///   TransactionType           (InventoryTransactionTypeEnum) → İşlem tipi
    ///   Quantity                  (int)                          → Miktar
    ///   SourceLocationType        (StockLocationTypeEnum?)       → Kaynak lokasyon tipi
    ///   SourceLocationId          (Guid?)                        → Kaynak depo veya araç Id'si
    ///   TargetLocationType        (StockLocationTypeEnum?)       → Hedef lokasyon tipi
    ///   TargetLocationId          (Guid?)                        → Hedef depo veya araç Id'si
    ///   RelatedMovementRequestId  (Guid?)                        → Bağlı talep Id'si
    ///   PerformedByUserId         (Guid?)                        → İşlemi başlatan kullanıcı Id'si
    ///   OccurredAt                (DateTime)                     → Hareket zamanı
    ///   Note                      (string?)                      → İşlem notu
    /// }
    /// </remarks>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<InventoryTransactionDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary>
    /// Envanter hareketi kayıtlarını sayfalı liste olarak getirir.
    /// </summary>
    /// <param name="input">Sayfalama parametreleri.</param>
    /// <remarks>
    /// Response {
    ///   ProductId                 (Guid)                         → Ürün Id'si
    ///   TransactionType           (InventoryTransactionTypeEnum) → İşlem tipi
    ///   Quantity                  (int)                          → Miktar
    ///   SourceLocationType        (StockLocationTypeEnum?)       → Kaynak lokasyon tipi
    ///   SourceLocationId          (Guid?)                        → Kaynak depo veya araç Id'si
    ///   TargetLocationType        (StockLocationTypeEnum?)       → Hedef lokasyon tipi
    ///   TargetLocationId          (Guid?)                        → Hedef depo veya araç Id'si
    ///   RelatedMovementRequestId  (Guid?)                        → Bağlı talep Id'si
    ///   PerformedByUserId         (Guid?)                        → İşlemi başlatan kullanıcı Id'si
    ///   OccurredAt                (DateTime)                     → Hareket zamanı
    ///   Note                      (string?)                      → İşlem notu
    /// }
    /// </remarks>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<PagedResultDto<InventoryTransactionDto>>> GetList([FromQuery] PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary>
    /// Yeni envanter hareketi kaydı oluşturur.
    /// </summary>
    /// <param name="input">Hareket bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   ProductId                (Guid)                         → Ürün Id'si
    ///   TransactionType          (InventoryTransactionTypeEnum) → İşlem tipi
    ///   Quantity                 (int)                          → Miktar
    ///   SourceLocationType       (StockLocationTypeEnum?)       → Kaynak lokasyon tipi
    ///   SourceLocationId         (Guid?)                        → Kaynak depo veya araç Id'si
    ///   TargetLocationType       (StockLocationTypeEnum?)       → Hedef lokasyon tipi
    ///   TargetLocationId         (Guid?)                        → Hedef depo veya araç Id'si
    ///   RelatedMovementRequestId (Guid?)                        → Bağlı talep Id'si
    ///   OccurredAt               (DateTime)                     → Hareket zamanı
    ///   Note                     (string?)                      → İşlem notu
    /// }
    /// Response {
    ///   ProductId                 (Guid)                         → Ürün Id'si
    ///   TransactionType           (InventoryTransactionTypeEnum) → İşlem tipi
    ///   Quantity                  (int)                          → Miktar
    ///   SourceLocationType        (StockLocationTypeEnum?)       → Kaynak lokasyon tipi
    ///   SourceLocationId          (Guid?)                        → Kaynak depo veya araç Id'si
    ///   TargetLocationType        (StockLocationTypeEnum?)       → Hedef lokasyon tipi
    ///   TargetLocationId          (Guid?)                        → Hedef depo veya araç Id'si
    ///   RelatedMovementRequestId  (Guid?)                        → Bağlı talep Id'si
    ///   PerformedByUserId         (Guid?)                        → İşlemi başlatan kullanıcı Id'si
    ///   OccurredAt                (DateTime)                     → Hareket zamanı
    ///   Note                      (string?)                      → İşlem notu
    /// }
    /// </remarks>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<InventoryTransactionDto>> Create([FromBody] CreateInventoryTransactionDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary>
    /// Birden fazla envanter hareketi kaydını toplu oluşturur.
    /// </summary>
    /// <param name="inputs">Hareket bilgileri listesi.</param>
    /// <remarks>
    /// Request {
    ///   ProductId                (Guid)                         → Ürün Id'si
    ///   TransactionType          (InventoryTransactionTypeEnum) → İşlem tipi
    ///   Quantity                 (int)                          → Miktar
    ///   SourceLocationType       (StockLocationTypeEnum?)       → Kaynak lokasyon tipi
    ///   SourceLocationId         (Guid?)                        → Kaynak depo veya araç Id'si
    ///   TargetLocationType       (StockLocationTypeEnum?)       → Hedef lokasyon tipi
    ///   TargetLocationId         (Guid?)                        → Hedef depo veya araç Id'si
    ///   RelatedMovementRequestId (Guid?)                        → Bağlı talep Id'si
    ///   OccurredAt               (DateTime)                     → Hareket zamanı
    ///   Note                     (string?)                      → İşlem notu
    /// }
    /// Response {
    ///   ProductId                 (Guid)                         → Ürün Id'si
    ///   TransactionType           (InventoryTransactionTypeEnum) → İşlem tipi
    ///   Quantity                  (int)                          → Miktar
    ///   SourceLocationType        (StockLocationTypeEnum?)       → Kaynak lokasyon tipi
    ///   SourceLocationId          (Guid?)                        → Kaynak depo veya araç Id'si
    ///   TargetLocationType        (StockLocationTypeEnum?)       → Hedef lokasyon tipi
    ///   TargetLocationId          (Guid?)                        → Hedef depo veya araç Id'si
    ///   RelatedMovementRequestId  (Guid?)                        → Bağlı talep Id'si
    ///   PerformedByUserId         (Guid?)                        → İşlemi başlatan kullanıcı Id'si
    ///   OccurredAt                (DateTime)                     → Hareket zamanı
    ///   Note                      (string?)                      → İşlem notu
    /// }
    /// </remarks>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<List<InventoryTransactionDto>>> CreateMany([FromBody] List<CreateInventoryTransactionDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary>
    /// Envanter hareketi kaydini guncelleme istegini reddeder.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <param name="input">Geriye donuk API uyumlulugu icin alinan, islenmeyen hareket bilgileri.</param>
    /// <remarks>
    /// InventoryTransaction append-only ledger kaydidir; mevcut kayitlar guncellenmez.
    /// Bu endpoint geriye donuk API uyumlulugu icin durur ve InventoryTransaction.ImmutableLedger hatasi dondurur.
    /// </remarks>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result<InventoryTransactionDto>> Update(Guid id, [FromBody] UpdateInventoryTransactionDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary>
    /// Envanter hareketi kaydini silme istegini reddeder.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <remarks>
    /// InventoryTransaction append-only ledger kaydidir; mevcut kayitlar soft/hard delete ile silinmez.
    /// Bu endpoint geriye donuk API uyumlulugu icin durur ve InventoryTransaction.ImmutableLedger hatasi dondurur.
    /// </remarks>
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}
