using System;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Stock;

/// <summary>
/// Urunun tek lokasyondaki stok ozetini donduren DTO.
/// </summary>
public class ProductStockLocationSummaryDto
{
    public InventoryLocationTypeEnum LocationType { get; set; } // Lokasyon tipi.
    public Guid? WarehouseSiteId { get; set; }                  // Depo Site Id'si.
    public Guid? VehicleId { get; set; }                        // Arac Id'si.
    public Guid? VehicleTaskId { get; set; }                    // Aktif gorev-arac atama Id'si.
    public Guid? InventoryTaskId { get; set; }                  // Aktif gorev Id'si.
    public int Quantity { get; set; }                           // Fiziksel miktar.
    public int ReservedQuantity { get; set; }                   // Rezerve miktar.
}
