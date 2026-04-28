using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation.Entities.Lookups;

/// <summary>
/// Organizasyondaki departman sozlugunu temsil eden lookup aggregate'i.
/// </summary>
public class Department : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public string Code { get; set; } = default!; // Departmanin kurumsal kodunu tasir.
    public string Name { get; set; } = default!; // Departmanin kullaniciya gorunen adini tasir.
    public Guid? TenantId { get; set; } // Departman verisini kiraci sinirinda tutar.

    protected Department() { }
    public Department(Guid id) : base(id) { }
}
