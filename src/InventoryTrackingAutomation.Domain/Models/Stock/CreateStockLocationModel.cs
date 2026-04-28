using System;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Models.Stock;

/// <summary>
/// Lokasyon bazli stok olusturma domain modeli.
/// </summary>
public class CreateStockLocationModel
{
    public Guid ProductId { get; set; }                       // Urun Id'si.
    public InventoryLocationTypeEnum LocationType { get; set; } // Lokasyon tipi.
    public Guid? WarehouseSiteId { get; set; }                // Depo ise Site Id'si.
    public Guid? VehicleId { get; set; }                      // Arac ise Vehicle Id'si.
    public int Quantity { get; set; }                         // Stok miktari.
    public int ReservedQuantity { get; set; }                 // Rezerve miktar.
}
