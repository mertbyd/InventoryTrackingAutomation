using System;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Stock;

/// <summary>
/// Lokasyon bazli stok response DTO'su.
/// </summary>
public class StockLocationDto : FullAuditedEntityDto<Guid>
{
    public Guid ProductId { get; set; }                       // Urun Id'si.
    public InventoryLocationTypeEnum LocationType { get; set; } // Lokasyon tipi.
    public Guid? WarehouseSiteId { get; set; }                // Depo Site Id'si.
    public Guid? VehicleId { get; set; }                      // Arac Id'si.
    public int Quantity { get; set; }                         // Stok miktari.
    public int ReservedQuantity { get; set; }                 // Rezerve miktar.
}
