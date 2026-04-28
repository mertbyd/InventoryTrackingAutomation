using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Models.Stock;

/// <summary>
/// Bir urunun tek lokasyondaki stok ozetini temsil eder.
/// </summary>
public class ProductStockLocationSummaryModel
{
    public StockLocationTypeEnum LocationType { get; set; } // Stokun bulundugu lokasyon tipi.
    public Guid? WarehouseId { get; set; }                  // Depo lokasyonu ise Warehouse Id'si.
    public Guid? VehicleId { get; set; }                        // Arac lokasyonu ise Vehicle Id'si.
    public Guid? VehicleTaskId { get; set; }                    // Arac aktif gorevdeyse VehicleTask Id'si.
    public Guid? InventoryTaskId { get; set; }                  // Aracin bagli oldugu gorev Id'si.
    public int Quantity { get; set; }                           // Lokasyondaki fiziksel miktar.
    public int ReservedQuantity { get; set; }                   // Lokasyondaki rezerve miktar.
}
