using System;
using InventoryTrackingAutomation.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Masters;

/// <summary>
/// Envanter saklanan depoyu temsil eden master aggregate.
/// </summary>
// islevi: Depo kodu, adi, adresi, sorumlusu ve aktiflik bilgisini tasir.
// sistemdeki gorevi: Stok cikis, hedef ve iade lokasyonu olarak kullanilan master veridir.
public class Warehouse : AuditedEntity<Guid>, IPassivable
{
    public string Code { get; set; } = default!; // Deponun kurumsal kodunu tasir.
    public string Name { get; set; } = default!; // Deponun operasyonlarda gorunen adini tasir.
    public string? Address { get; set; } // Depoya ulasim icin adres baglamini tasir.
    public Guid? ManagerWorkerId { get; set; } // Depodan sorumlu calisan baglamini tasir.
    public bool IsActive { get; set; } // Deponun operasyonlarda kullanilip kullanilamayacagini belirler.

    public virtual Worker? ManagerWorker { get; set; }
    protected Warehouse() { }
    public Warehouse(Guid id) : base(id) { }
}

