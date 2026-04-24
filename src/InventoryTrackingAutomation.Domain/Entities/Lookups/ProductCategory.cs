using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Lookups;

/// <summary>
/// Ürünlerin hiyerarşik kategori yapısını temsil eden lookup entity'si.
/// </summary>
public class ProductCategory : FullAuditedEntity<Guid>
{
    public string Code { get; set; }      // Kategorinin benzersiz kodu. Örnek: "CAT-001"
    public string Name { get; set; }      // Kategorinin adı. Örnek: "Elektrik Malzemeleri"
    public Guid? ParentId { get; set; }   // Üst kategori kimliği (hiyerarşi için). Örnek: "Ana Kategori" Id'si

    protected ProductCategory() { }
    public ProductCategory(Guid id) : base(id) { }
}
