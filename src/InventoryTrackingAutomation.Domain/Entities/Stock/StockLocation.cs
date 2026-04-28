using System;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Stock;

/// <summary>
/// Urun stok miktarini depo veya arac lokasyonu bazinda tutar.
/// </summary>
public class StockLocation : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid ProductId { get; set; }                       // Stoku takip edilen urun Id'si.
    public InventoryLocationTypeEnum LocationType { get; set; } // Stokun bulundugu lokasyon tipi.
    public Guid LocationId { get; set; }                      // Lokasyon tipinin isaret ettigi depo veya arac Id'si.
    public int Quantity { get; set; }                         // Kullanilabilir fiziksel miktar.
    public int ReservedQuantity { get; set; }                 // Rezerve edilen miktar.
    public Guid? TenantId { get; set; }                       // ABP tenant izolasyonu icin kiraci Id'si.

    protected StockLocation() { }
    public StockLocation(Guid id) : base(id) { }
}
