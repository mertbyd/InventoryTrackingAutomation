using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Inventory;

/// <summary>
/// Urunun tek lokasyondaki stok ozetini donduren DTO.
/// </summary>
//işlevi: ProductStockLocationSummary verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class ProductStockLocationSummaryDto
{
    public StockLocationTypeEnum LocationType { get; set; } // Lokasyon tipi.
    public Guid? WarehouseId { get; set; }                  // Depo Warehouse Id'si.
    public Guid? VehicleId { get; set; }                        // Arac Id'si.
    public Guid? VehicleTaskId { get; set; }                    // Aktif gorev-arac atama Id'si.
    public Guid? InventoryTaskId { get; set; }                  // Aktif gorev Id'si.
    public int Quantity { get; set; }                           // Fiziksel miktar.
    public int ReservedQuantity { get; set; }                   // Rezerve miktar.
}
