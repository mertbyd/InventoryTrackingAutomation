using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Lookups;

/// <summary>
/// Organizasyondaki departmanları temsil eden lookup entity'si.
/// </summary>
public class Department : FullAuditedEntity<Guid>
{
    public string Code { get; set; }  // Departmanın benzersiz kodu. Örnek: "DEP-IT"
    public string Name { get; set; }  // Departmanın adı. Örnek: "Bilgi Teknolojileri"

    protected Department() { }
    public Department(Guid id) : base(id) { }
}
