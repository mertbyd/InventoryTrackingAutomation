using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using System;
using Volo.Abp.Application.Dtos;
using InventoryTrackingAutomation.Dtos.Common;

namespace InventoryTrackingAutomation.Dtos.Inventory;

//işlevi: InventoryTransaction verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class InventoryTransactionDto : EnrichedEntityDto<Guid>
{
    /// <summary>
    /// Urun Id&apos;si.
    /// </summary>
    public Guid ProductId { get; set; }                                  // Urun Id'si.
    /// <summary>
    /// Islem tipi.
    /// </summary>
    public InventoryTransactionTypeEnum TransactionType { get; set; }    // Islem tipi.
    /// <summary>
    /// Miktar.
    /// </summary>
    public int Quantity { get; set; }                                    // Miktar.
    /// <summary>
    /// Kaynak lokasyon tipi.
    /// </summary>
    public StockLocationTypeEnum? SourceLocationType { get; set; }   // Kaynak lokasyon tipi.
    /// <summary>
    /// Kaynak depo veya arac Id&apos;si.
    /// </summary>
    public Guid? SourceLocationId { get; set; }                          // Kaynak depo veya arac Id'si.
    /// <summary>
    /// Hedef lokasyon tipi.
    /// </summary>
    public StockLocationTypeEnum? TargetLocationType { get; set; }   // Hedef lokasyon tipi.
    /// <summary>
    /// Hedef depo veya arac Id&apos;si.
    /// </summary>
    public Guid? TargetLocationId { get; set; }                          // Hedef depo veya arac Id'si.
    /// <summary>
    /// Bagli talep Id&apos;si.
    /// </summary>
    public Guid? RelatedMovementRequestId { get; set; }                  // Bagli talep Id'si.
    /// <summary>
    /// Islemi baslatan kullanici Id&apos;si.
    /// </summary>
    public Guid? PerformedByUserId { get; set; }                         // Islemi baslatan kullanici Id'si.
    /// <summary>
    /// Hareket zamani.
    /// </summary>
    public DateTime OccurredAt { get; set; }                             // Hareket zamani.
    /// <summary>
    /// Islem notu.
    /// </summary>
    public string? Note { get; set; }                                    // Islem notu.
    public string ProductName { get; set; }
    public string SourceLocationName { get; set; }
    public string TargetLocationName { get; set; }

    public string RelatedMovementRequestName { get; set; }
    public string PerformedByUserName { get; set; }
}

