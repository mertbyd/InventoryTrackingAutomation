using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation.Entities.Masters;

/// <summary>
/// Stokta takip edilen urunu temsil eden master aggregate.
/// </summary>
public class Product : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public string Code { get; set; } = default!; // Urunun kurumsal kodunu tasir.
    public string Name { get; set; } = default!; // Urunun operasyonlarda gorunen adini tasir.
    public Guid? CategoryId { get; set; } // Urunun bagli oldugu kategori baglamini tasir.
    public UnitTypeEnum BaseUnit { get; set; } // Urunun stok olcum birimini belirler.
    public bool IsActive { get; set; } // Urunun operasyonlarda kullanilip kullanilamayacagini belirler.
    public bool IsSerializable { get; set; } // Urunun seri bazli takip gerektirip gerektirmedigini belirler.
    public Guid? TenantId { get; set; } // Urun katalog verisini kiraci sinirinda tutar.

    protected Product() { }
    public Product(Guid id) : base(id) { }
}
