using System;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation.Entities.Stock;

/// <summary>
/// Tum depo/arac stok hareketlerinin kronolojik denetim kaydini temsil eder.
/// </summary>
public class InventoryTransaction : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid ProductId { get; set; }                                  // Hareket eden urun Id'si.
    public InventoryTransactionTypeEnum TransactionType { get; set; }    // Transfer islem tipi.
    public int Quantity { get; set; }                                    // Transfer miktari.
    public InventoryLocationTypeEnum? SourceLocationType { get; set; }   // Kaynak lokasyon tipi.
    public Guid? SourceLocationId { get; set; }                          // Kaynak depo veya arac Id'si.
    public InventoryLocationTypeEnum? TargetLocationType { get; set; }   // Hedef lokasyon tipi.
    public Guid? TargetLocationId { get; set; }                          // Hedef depo veya arac Id'si.
    public Guid? RelatedMovementRequestId { get; set; }                  // Isleme sebep olan onayli talep Id'si.
    public Guid? RelatedTaskId { get; set; }                             // Gorev baglami Id'si.
    public Guid? PerformedByUserId { get; set; }                         // Islemi baslatan kullanici Id'si.
    public DateTime OccurredAt { get; set; }                             // Hareketin gerceklestigi zaman.
    public string? Note { get; set; }                                    // Islem aciklamasi.
    public Guid? TenantId { get; set; }                                  // ABP tenant izolasyonu icin kiraci Id'si.

    protected InventoryTransaction() { }
    public InventoryTransaction(Guid id) : base(id) { }
}
