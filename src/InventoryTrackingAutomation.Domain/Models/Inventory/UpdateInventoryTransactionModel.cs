using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Models.Inventory;

/// <summary>
/// Envanter hareketi guncelleme domain modeli.
/// </summary>
public class UpdateInventoryTransactionModel
{
    public Guid ProductId { get; set; }                                  // Hareket eden urun Id'si.
    public InventoryTransactionTypeEnum TransactionType { get; set; }    // Islem tipi.
    public int Quantity { get; set; }                                    // Hareket miktari.
    public StockLocationTypeEnum? SourceLocationType { get; set; }   // Kaynak lokasyon tipi.
    public Guid? SourceLocationId { get; set; }                          // Kaynak depo veya arac Id'si.
    public StockLocationTypeEnum? TargetLocationType { get; set; }   // Hedef lokasyon tipi.
    public Guid? TargetLocationId { get; set; }                          // Hedef depo veya arac Id'si.
    public Guid? RelatedMovementRequestId { get; set; }                  // Bagli talep Id'si.
    public Guid? RelatedTaskId { get; set; }                             // Bagli gorev Id'si.
    public Guid? PerformedByUserId { get; set; }                         // Islemi baslatan kullanici Id'si.
    public DateTime OccurredAt { get; set; }                             // Hareket zamani.
    public string? Note { get; set; }                                    // Islem notu.
}
