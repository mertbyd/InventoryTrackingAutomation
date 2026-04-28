using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation.Entities.Lookups;

/// <summary>
/// Urunlerin hiyerarsik kategori sozlugunu temsil eden lookup aggregate'i.
/// </summary>
public class ProductCategory : FullAuditedEntity<Guid>
{
    public string Code { get; set; } = default!; // Kategorinin kurumsal kodunu tasir.
    public string Name { get; set; } = default!; // Kategorinin kullaniciya gorunen adini tasir.
    public Guid? ParentId { get; set; } // Hiyerarside ust kategori baglamini tasir.

    protected ProductCategory() { }
    public ProductCategory(Guid id) : base(id) { }
}
