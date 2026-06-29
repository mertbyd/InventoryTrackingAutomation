using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using System;

namespace InventoryTrackingAutomation.Dtos.Inventory;

//işlevi: ProductStockLocationSummary verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class ProductStockLocationSummaryDto
{
    /// <summary>
    /// Lokasyon tipi.
    /// </summary>
    public StockLocationTypeEnum LocationType { get; set; } // Lokasyon tipi.
    /// <summary>
    /// Depo Warehouse Id&apos;si.
    /// </summary>
    public Guid? WarehouseId { get; set; }                  // Depo Warehouse Id'si.
    /// <summary>
    /// Arac Id&apos;si.
    /// </summary>
    public Guid? VehicleId { get; set; }                        // Arac Id'si.
    /// <summary>
    /// Aktif gorev-arac atama Id&apos;si.
    /// </summary>
    public Guid? VehicleTaskId { get; set; }                    // Aktif gorev-arac atama Id'si.
    /// <summary>
    /// Aktif operasyon isi Id&apos;si.
    /// </summary>
    public Guid? TaskId { get; set; }                           // Aktif operasyon isi Id'si.
    /// <summary>
    /// Fiziksel miktar.
    /// </summary>
    public int Quantity { get; set; }                           // Fiziksel miktar.
    /// <summary>
    /// Rezerve miktar.
    /// </summary>
    public int ReservedQuantity { get; set; }                   // Rezerve miktar.
}
