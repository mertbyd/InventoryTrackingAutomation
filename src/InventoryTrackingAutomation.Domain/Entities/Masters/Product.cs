using InventoryTrackingAutomation.Entities.Lookups;
using System;
using InventoryTrackingAutomation.Entities;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Masters;

/// <summary>
/// Stokta takip edilen urunu temsil eden master aggregate.
/// </summary>
// islevi: Stokta takip edilen urunun kimlik, kategori, birim ve aktiflik bilgilerini tasir.
// sistemdeki gorevi: Operasyonel stok hareketlerinin uzerinden yurutuldugu temel master veridir.
public class Product : AuditedEntity<Guid>, IPassivable
{
    public string Code { get; set; } = default!; // Urunun kurumsal kodunu tasir.
    public string Name { get; set; } = default!; // Urunun operasyonlarda gorunen adini tasir.
    public Guid? CategoryId { get; set; } // Urunun bagli oldugu kategori baglamini tasir.
    public Guid UnitTypeId { get; set; } // Urunun stok olcum birimini (Lookup FK) belirler.
    public bool IsActive { get; set; } // Urunun operasyonlarda kullanilip kullanilamayacagini belirler.
    public bool IsSerializable { get; set; } // Urunun seri bazli takip gerektirip gerektirmedigini belirler.

    public virtual ProductCategory? Category { get; set; }

    public virtual UnitType UnitType { get; set; }
    protected Product() { }
    public Product(Guid id) : base(id) { }
}


