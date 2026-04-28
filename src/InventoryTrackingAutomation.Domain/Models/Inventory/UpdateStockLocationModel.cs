using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Models.Inventory;

/// <summary>
/// Lokasyon bazli stok guncelleme domain modeli.
/// </summary>
public class UpdateStockLocationModel
{
    public Guid ProductId { get; set; }                       // Urun Id'si.
    public StockLocationTypeEnum LocationType { get; set; } // Lokasyon tipi.
    public Guid LocationId { get; set; }                      // Depo veya arac Id'si.
    public int Quantity { get; set; }                         // Stok miktari.
    public int ReservedQuantity { get; set; }                 // Rezerve miktar.
}
