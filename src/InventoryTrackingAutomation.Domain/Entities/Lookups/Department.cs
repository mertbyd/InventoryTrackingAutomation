using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation.Entities.Lookups;

/// <summary>
/// Organizasyondaki departman sozlugunu temsil eden lookup aggregate'i.
/// </summary>
public class Department : FullAuditedEntity<Guid>
{
    public string Code { get; set; } = default!; // Departmanin kurumsal kodunu tasir.
    public string Name { get; set; } = default!; // Departmanin kullaniciya gorunen adini tasir.

    protected Department() { }
    public Department(Guid id) : base(id) { }
}
