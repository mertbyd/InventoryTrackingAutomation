using System;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Models.Stock;

/// <summary>
/// Depo ve arac arasindaki stok transferinin domain girdisini temsil eder.
/// </summary>
public class StockTransferModel
{
    public Guid ProductId { get; set; } // Transfer edilen urun baglamini tasir.
    public int Quantity { get; set; } // Transfer miktarini tasir.
    public InventoryLocationTypeEnum SourceLocationType { get; set; } // Kaynak lokasyonun depo mu arac mi oldugunu belirler.
    public Guid SourceLocationId { get; set; } // Kaynak depo veya arac kimligini tasir.
    public InventoryLocationTypeEnum TargetLocationType { get; set; } // Hedef lokasyonun depo mu arac mi oldugunu belirler.
    public Guid TargetLocationId { get; set; } // Hedef depo veya arac kimligini tasir.
    public InventoryTransactionTypeEnum TransactionType { get; set; } // Ledger kaydinin islem tipini belirler.
    public Guid? RelatedMovementRequestId { get; set; } // Transferi doguran talep baglamini tasir.
    public Guid? RelatedTaskId { get; set; } // Transferin bagli oldugu saha gorevi baglamini tasir.
    public Guid? PerformedByUserId { get; set; } // Transferi baslatan kullanici baglamini tasir.
    public string? Note { get; set; } // Transfer icin operasyonel aciklama baglamini tasir.
}
