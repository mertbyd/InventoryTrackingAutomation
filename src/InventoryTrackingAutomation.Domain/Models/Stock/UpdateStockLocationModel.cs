using System;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Models.Stock;

/// <summary>
/// Lokasyon bazli stok guncelleme domain modeli.
/// </summary>
public class UpdateStockLocationModel
{
    public Guid ProductId { get; set; }                       // Urun Id'si.
    public InventoryLocationTypeEnum LocationType { get; set; } // Lokasyon tipi.
    public Guid LocationId { get; set; }                      // Depo veya arac Id'si.
    public int Quantity { get; set; }                         // Stok miktari.
    public int ReservedQuantity { get; set; }                 // Rezerve miktar.
}
