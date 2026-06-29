using System;
using Volo.Abp.Domain.Entities;

namespace InventoryTrackingAutomation.Entities.Lookups;

/// <summary>
/// Organizasyondaki departman sozlugunu temsil eden lookup aggregate'i.
/// </summary>
public class Department : IEntity<Guid>
{
    public Guid Id { get; protected set; } // Departmanin benzersiz kimligini tasir.
    public string Code { get; set; } = default!; // Departmanin kurumsal kodunu tasir.
    public string Name { get; set; } = default!; // Departmanin kullaniciya gorunen adini tasir.

    protected Department() { }
    public Department(Guid id)
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
