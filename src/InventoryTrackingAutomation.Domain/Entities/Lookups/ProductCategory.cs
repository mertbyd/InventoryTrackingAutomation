using System;
using Volo.Abp.Domain.Entities;

namespace InventoryTrackingAutomation.Entities.Lookups;

/// <summary>
/// Urunlerin hiyerarsik kategori sozlugunu temsil eden lookup aggregate'i.
/// </summary>
public class ProductCategory : IEntity<Guid>
{
    public Guid Id { get; protected set; } // Kategorinin benzersiz kimligini tasir.
    public string Code { get; set; } = default!; // Kategorinin kurumsal kodunu tasir.
    public string Name { get; set; } = default!; // Kategorinin kullaniciya gorunen adini tasir.
    public Guid? ParentId { get; set; } // Hiyerarside ust kategori baglamini tasir.

    public virtual ProductCategory? Parent { get; set; }
    protected ProductCategory() { }
    public ProductCategory(Guid id)
    {
        Id = id;
    }

    public object[] GetKeys()
    {
        return new object[] { Id };
    }

    public string GetObjectKey()
    {
        return Id.ToString();
    }
}

