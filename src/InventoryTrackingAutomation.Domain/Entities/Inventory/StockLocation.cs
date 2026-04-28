using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation.Entities.Inventory;

/// <summary>
/// Bir urunun depo veya arac uzerindeki stok bakiyesini temsil eden aggregate.
/// </summary>
public class StockLocation : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid ProductId { get; set; } // Stoku izlenen urun baglamini tasir.
    public StockLocationTypeEnum LocationType { get; set; } // Fiziksel lokasyonun depo mu arac mi oldugunu belirler.
    public Guid LocationId { get; set; } // Lokasyon tipinin isaret ettigi depo veya arac kimligini tasir.
    public int Quantity { get; set; } // Fiziksel kullanilabilir stok miktarini tasir.
    public int ReservedQuantity { get; set; } // Depo stoklarinda ayrilmis miktari tasir.
    public Guid? TenantId { get; set; } // Stok bakiyesini kiraci sinirinda tutar.

    protected StockLocation() { }
    public StockLocation(Guid id) : base(id) { }
}
