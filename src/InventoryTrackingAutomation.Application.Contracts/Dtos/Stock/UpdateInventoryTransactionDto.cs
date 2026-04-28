using System;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Stock;

/// <summary>
/// Envanter hareketi guncelleme request DTO'su.
/// </summary>
public class UpdateInventoryTransactionDto
{
    public Guid ProductId { get; set; }                                  // Urun Id'si.
    public InventoryTransactionTypeEnum TransactionType { get; set; }    // Islem tipi.
    public int Quantity { get; set; }                                    // Miktar.
    public InventoryLocationTypeEnum? SourceLocationType { get; set; }   // Kaynak lokasyon tipi.
    public Guid? SourceLocationId { get; set; }                          // Kaynak depo veya arac Id'si.
    public InventoryLocationTypeEnum? TargetLocationType { get; set; }   // Hedef lokasyon tipi.
    public Guid? TargetLocationId { get; set; }                          // Hedef depo veya arac Id'si.
    public Guid? RelatedMovementRequestId { get; set; }                  // Bagli talep Id'si.
    public Guid? RelatedTaskId { get; set; }                             // Bagli gorev Id'si.
    public DateTime OccurredAt { get; set; }                             // Hareket zamani.
    public string? Note { get; set; }                                    // Islem notu.
}
