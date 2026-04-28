using System;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Stock;

/// <summary>
/// Lokasyon bazli stok guncelleme request DTO'su.
/// </summary>
public class UpdateStockLocationDto
{
    public Guid ProductId { get; set; }                       // Urun Id'si.
    public InventoryLocationTypeEnum LocationType { get; set; } // Lokasyon tipi.
    public Guid? WarehouseSiteId { get; set; }                // Depo Site Id'si.
    public Guid? VehicleId { get; set; }                      // Arac Id'si.
    public int Quantity { get; set; }                         // Stok miktari.
    public int ReservedQuantity { get; set; }                 // Rezerve miktar.
}
