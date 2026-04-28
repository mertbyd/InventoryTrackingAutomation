using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation.Entities.Masters;

/// <summary>
/// Envanter saklanan depoyu temsil eden master aggregate.
/// </summary>
public class Warehouse : FullAuditedEntity<Guid>
{
    public string Code { get; set; } = default!; // Deponun kurumsal kodunu tasir.
    public string Name { get; set; } = default!; // Deponun operasyonlarda gorunen adini tasir.
    public string? Address { get; set; } // Depoya ulasim icin adres baglamini tasir.
    public Guid? ManagerWorkerId { get; set; } // Depodan sorumlu calisan baglamini tasir.
    public bool IsActive { get; set; } // Deponun operasyonlarda kullanilip kullanilamayacagini belirler.

    protected Warehouse() { }
    public Warehouse(Guid id) : base(id) { }
}
