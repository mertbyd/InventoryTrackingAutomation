using System;
using InventoryTrackingAutomation.Entities;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Masters;

/// <summary>
/// Calisan ve organizasyon hiyerarsisini temsil eden master aggregate.
/// </summary>
// islevi: Identity kullanicisinin calisan karsiligini ve organizasyonel baglamini tasir.
// sistemdeki gorevi: Gorev sorumlusu, talep sahibi ve onayci gibi operasyonel rolleri temsil eder.
public class Worker : AuditedEntity<Guid>, IPassivable
{
    public Guid UserId { get; set; } // Identity kullanicisinin calisan karsiligini tasir.
    public string RegistrationNumber { get; set; } = default!; // Calisanin kurumsal sicil numarasini tasir.
    public Guid WorkerTypeId { get; set; } // Calisanin rol tipini (Lookup FK) belirler.
    public Guid? DepartmentId { get; set; } // Calisanin organizasyonel departman baglamini tasir.
    public Guid? DefaultWarehouseId { get; set; } // Calisanin varsayilan depo baglamini tasir.
    public Guid? ManagerId { get; set; } // Calisanin organizasyonel yonetici baglamini tasir.
    public bool IsActive { get; set; } // Calisanin operasyonlarda kullanilip kullanilamayacagini belirler.

    public virtual InventoryTrackingAutomation.Entities.Lookups.WorkerType WorkerType { get; set; }
    public virtual InventoryTrackingAutomation.Entities.Lookups.Department? Department { get; set; }
    public virtual Warehouse? DefaultWarehouse { get; set; }
    public virtual Worker? Manager { get; set; }
    protected Worker() { }
    public Worker(Guid id) : base(id) { }
}

