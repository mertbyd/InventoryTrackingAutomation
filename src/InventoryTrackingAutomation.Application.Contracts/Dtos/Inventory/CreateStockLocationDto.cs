using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using System;

namespace InventoryTrackingAutomation.Dtos.Inventory;

//işlevi: CreateStockLocation verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class CreateStockLocationDto
{
    /// <summary>
    /// Urun Id&apos;si.
    /// </summary>
    public Guid ProductId { get; set; }                       // Urun Id'si.
    /// <summary>
    /// Lokasyon tipi.
    /// </summary>
    public StockLocationTypeEnum LocationType { get; set; } // Lokasyon tipi.
    /// <summary>
    /// Depo veya arac Id&apos;si.
    /// </summary>
    public Guid LocationId { get; set; }                      // Depo veya arac Id'si.
    /// <summary>
    /// Stok miktari.
    /// </summary>
    public int Quantity { get; set; }                         // Stok miktari.
    /// <summary>
    /// Rezerve miktar.
    /// </summary>
    public int ReservedQuantity { get; set; }                 // Rezerve miktar.
}
