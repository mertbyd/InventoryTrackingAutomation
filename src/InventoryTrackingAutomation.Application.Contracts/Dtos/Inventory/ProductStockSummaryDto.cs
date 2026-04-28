using System;
using System.Collections.Generic;

namespace InventoryTrackingAutomation.Dtos.Inventory;

/// <summary>
/// Urun stok ozetini depo/arac/gorev dagilimi ile donduren DTO.
/// </summary>
public class ProductStockSummaryDto
{
    public Guid ProductId { get; set; }                                          // Ozetlenen urun Id'si.
    public int TotalQuantity { get; set; }                                       // Toplam stok miktari.
    public int WarehouseQuantity { get; set; }                                   // Depolardaki toplam miktar.
    public int VehicleQuantity { get; set; }                                     // Araclardaki toplam miktar.
    public int ActiveTaskQuantity { get; set; }                                  // Aktif gorevlerdeki toplam miktar.
    public List<ProductStockLocationSummaryDto> Locations { get; set; } = new(); // Lokasyon detaylari.
}
