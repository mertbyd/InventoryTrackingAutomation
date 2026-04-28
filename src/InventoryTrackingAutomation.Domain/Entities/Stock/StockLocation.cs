using System;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Stock;

/// <summary>
/// Urun stok miktarini depo veya arac lokasyonu bazinda tutar.
/// </summary>
public class StockLocation : FullAuditedEntity<Guid>
{
    public Guid ProductId { get; set; }                       // Stoku takip edilen urun Id'si.
    public InventoryLocationTypeEnum LocationType { get; set; } // Stokun bulundugu lokasyon tipi.
    public Guid? WarehouseSiteId { get; set; }                // Depo lokasyonu ise Site Id'si.
    public Guid? VehicleId { get; set; }                      // Arac lokasyonu ise Vehicle Id'si.
    public int Quantity { get; set; }                         // Kullanilabilir fiziksel miktar.
    public int ReservedQuantity { get; set; }                 // Rezerve edilen miktar.

    protected StockLocation() { }
    public StockLocation(Guid id) : base(id) { }
}
