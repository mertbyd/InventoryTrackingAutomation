using InventoryTrackingAutomation.Entities.Masters;
using System;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation.Entities.Inventory;

/// <summary>
/// Depo ve arac stok hareketlerinin degistirilemez ledger kaydini temsil eden aggregate.
/// </summary>
// islevi: Stok hareketlerini append-only defter kaydi olarak saklar.
// sistemdeki gorevi: Stok degisimlerinin kaynak, hedef, miktar ve zaman bilgisini sonradan degistirilmeden izlenebilir tutar.
public class InventoryTransaction : CreationAuditedEntity<Guid>
{
    public Guid ProductId { get; set; } // Hareket eden urun baglamini tasir.
    public InventoryTransactionTypeEnum TransactionType { get; set; } // Hareketin yon ve sebep tipini belirler.
    public int Quantity { get; set; } // Transfer edilen miktari tasir.
    public StockLocationTypeEnum? SourceLocationType { get; set; } // Kaynak lokasyonun depo mu arac mi oldugunu belirler.
    public Guid? SourceLocationId { get; set; } // Kaynak depo veya arac kimligini tasir.
    public StockLocationTypeEnum? TargetLocationType { get; set; } // Hedef lokasyonun depo mu arac mi oldugunu belirler.
    public Guid? TargetLocationId { get; set; } // Hedef depo veya arac kimligini tasir.
    public Guid? RelatedMovementRequestId { get; set; } // Hareketi doguran talep baglamini tasir.
    public Guid? PerformedByUserId { get; set; } // Hareketi baslatan kullanici baglamini tasir.
    public DateTime OccurredAt { get; set; } // Hareketin gerceklestigi zamani tasir.
    public string? Note { get; set; } // Hareket icin operasyonel aciklama baglamini tasir.

    public virtual Product Product { get; set; }
    public virtual MovementRequest? RelatedMovementRequest { get; set; }
    protected InventoryTransaction() { }
    public InventoryTransaction(Guid id) : base(id) { }
}


