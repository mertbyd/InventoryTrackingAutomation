using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Inventory;

/// <summary>
/// Lokasyon bazli stok response DTO'su.
/// </summary>
//işlevi: StockLocation verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class StockLocationDto : FullAuditedEntityDto<Guid>
{
    public Guid ProductId { get; set; }                       // Urun Id'si.
    public StockLocationTypeEnum LocationType { get; set; } // Lokasyon tipi.
    public Guid LocationId { get; set; }                      // Depo veya arac Id'si.
    public int Quantity { get; set; }                         // Stok miktari.
    public int ReservedQuantity { get; set; }                 // Rezerve miktar.
}
