using System;
using System.Collections.Generic;

namespace InventoryTrackingAutomation.Models.Inventory;

/// <summary>
/// Urunun depo, arac ve gorev bazli toplam stok ozetini temsil eder.
/// </summary>
public class ProductStockSummaryModel
{
    public Guid ProductId { get; set; }                                            // Ozetlenen urun Id'si.
    public int TotalQuantity { get; set; }                                         // Tum lokasyonlardaki toplam miktar.
    public int WarehouseQuantity { get; set; }                                     // Depolardaki toplam miktar.
    public int VehicleQuantity { get; set; }                                       // Araclardaki toplam miktar.
    public int ActiveTaskQuantity { get; set; }                                    // Aktif goreve bagli araclarda bulunan miktar.
    public List<ProductStockLocationSummaryModel> Locations { get; set; } = new(); // Lokasyon bazli detaylar.
}
