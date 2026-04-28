using System;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Stock;

/// <summary>
/// Tum depo/arac stok hareketlerinin kronolojik denetim kaydini temsil eder.
/// </summary>
public class InventoryTransaction : FullAuditedEntity<Guid>
{
    public Guid ProductId { get; set; }                                  // Hareket eden urun Id'si.
    public InventoryTransactionTypeEnum TransactionType { get; set; }    // Transfer islem tipi.
    public int Quantity { get; set; }                                    // Transfer miktari.
    public InventoryLocationTypeEnum? SourceLocationType { get; set; }   // Kaynak lokasyon tipi.
    public Guid? SourceWarehouseSiteId { get; set; }                     // Kaynak depo Site Id'si.
    public Guid? SourceVehicleId { get; set; }                           // Kaynak arac Id'si.
    public InventoryLocationTypeEnum? TargetLocationType { get; set; }   // Hedef lokasyon tipi.
    public Guid? TargetWarehouseSiteId { get; set; }                     // Hedef depo Site Id'si.
    public Guid? TargetVehicleId { get; set; }                           // Hedef arac Id'si.
    public Guid? MovementRequestId { get; set; }                         // Isleme sebep olan onayli talep Id'si.
    public Guid? VehicleTaskId { get; set; }                             // Arac gorev baglami Id'si.
    public DateTime OccurredAt { get; set; }                             // Hareketin gerceklestigi zaman.
    public string? Note { get; set; }                                    // Islem aciklamasi.

    protected InventoryTransaction() { }
    public InventoryTransaction(Guid id) : base(id) { }
}
