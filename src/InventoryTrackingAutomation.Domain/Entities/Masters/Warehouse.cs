using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation.Entities.Masters;

/// <summary>
/// Envanter saklanan depoyu temsil eden master data entity'si.
/// </summary>
public class Warehouse : FullAuditedEntity<Guid>, IMultiTenant
{
    public string Code { get; set; } = default!;      // Deponun benzersiz is kodu.
    public string Name { get; set; } = default!;      // Depo adi.
    public string? Address { get; set; }              // Depoya ulasim icin adres bilgisi.
    public Guid? ManagerWorkerId { get; set; }        // Depodan sorumlu calisan.
    public bool IsActive { get; set; }                // Operasyonlarda kullanilabilirlik durumu.
    public Guid? TenantId { get; set; }               // ABP tenant izolasyonu.

    protected Warehouse() { }
    public Warehouse(Guid id) : base(id) { }
}
