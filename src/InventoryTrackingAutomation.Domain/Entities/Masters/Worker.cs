using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation.Entities.Masters;

/// <summary>
/// Calisan ve organizasyon hiyerarsisini temsil eden master aggregate.
/// </summary>
public class Worker : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid UserId { get; set; } // Identity kullanicisinin calisan karsiligini tasir.
    public string RegistrationNumber { get; set; } = default!; // Calisanin kurumsal sicil numarasini tasir.
    public WorkerTypeEnum WorkerType { get; set; } // Calisanin onay ve gorevlerdeki rol tipini belirler.
    public Guid? DepartmentId { get; set; } // Calisanin organizasyonel departman baglamini tasir.
    public Guid? DefaultWarehouseId { get; set; } // Calisanin varsayilan depo baglamini tasir.
    public Guid? ManagerId { get; set; } // Calisanin organizasyonel yonetici baglamini tasir.
    public bool IsActive { get; set; } // Calisanin operasyonlarda kullanilip kullanilamayacagini belirler.
    public Guid? TenantId { get; set; } // Calisan verisini kiraci sinirinda tutar.

    protected Worker() { }
    public Worker(Guid id) : base(id) { }
}
