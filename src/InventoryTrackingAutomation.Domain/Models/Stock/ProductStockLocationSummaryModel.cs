using System;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Models.Stock;

/// <summary>
/// Bir urunun tek lokasyondaki stok ozetini temsil eder.
/// </summary>
public class ProductStockLocationSummaryModel
{
    public InventoryLocationTypeEnum LocationType { get; set; } // Stokun bulundugu lokasyon tipi.
    public Guid? WarehouseSiteId { get; set; }                  // Depo lokasyonu ise Site Id'si.
    public Guid? VehicleId { get; set; }                        // Arac lokasyonu ise Vehicle Id'si.
    public Guid? VehicleTaskId { get; set; }                    // Arac aktif gorevdeyse VehicleTask Id'si.
    public Guid? InventoryTaskId { get; set; }                  // Aracin bagli oldugu gorev Id'si.
    public int Quantity { get; set; }                           // Lokasyondaki fiziksel miktar.
    public int ReservedQuantity { get; set; }                   // Lokasyondaki rezerve miktar.
}
