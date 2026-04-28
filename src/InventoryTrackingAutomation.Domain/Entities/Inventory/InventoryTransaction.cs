using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation.Entities.Inventory;

/// <summary>
/// Depo ve arac stok hareketlerinin degistirilemez ledger kaydini temsil eden aggregate.
/// </summary>
public class InventoryTransaction : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid ProductId { get; set; } // Hareket eden urun baglamini tasir.
    public InventoryTransactionTypeEnum TransactionType { get; set; } // Hareketin yon ve sebep tipini belirler.
    public int Quantity { get; set; } // Transfer edilen miktari tasir.
    public StockLocationTypeEnum? SourceLocationType { get; set; } // Kaynak lokasyonun depo mu arac mi oldugunu belirler.
    public Guid? SourceLocationId { get; set; } // Kaynak depo veya arac kimligini tasir.
    public StockLocationTypeEnum? TargetLocationType { get; set; } // Hedef lokasyonun depo mu arac mi oldugunu belirler.
    public Guid? TargetLocationId { get; set; } // Hedef depo veya arac kimligini tasir.
    public Guid? RelatedMovementRequestId { get; set; } // Hareketi doguran talep baglamini tasir.
    public Guid? RelatedTaskId { get; set; } // Hareketin bagli oldugu saha gorevi baglamini tasir.
    public Guid? PerformedByUserId { get; set; } // Hareketi baslatan kullanici baglamini tasir.
    public DateTime OccurredAt { get; set; } // Hareketin gerceklestigi zamani tasir.
    public string? Note { get; set; } // Hareket icin operasyonel aciklama baglamini tasir.
    public Guid? TenantId { get; set; } // Ledger kaydini kiraci sinirinda tutar.

    protected InventoryTransaction() { }
    public InventoryTransaction(Guid id) : base(id) { }
}
