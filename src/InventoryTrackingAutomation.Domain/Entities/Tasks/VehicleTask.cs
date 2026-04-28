using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation.Entities.Tasks;

/// <summary>
/// Bir aracin belirli bir envanter gorevine atanma gecmisini temsil eden aggregate.
/// </summary>
public class VehicleTask : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid VehicleId { get; set; } // Goreve atanan arac baglamini tasir.
    public Guid InventoryTaskId { get; set; } // Aracin bagli oldugu gorev baglamini tasir.
    public Guid DriverWorkerId { get; set; } // Gorev sirasinda araci kullanan calisan baglamini tasir.
    public DateTime AssignedAt { get; set; } // Aracin goreve dahil edildigi zamani tasir.
    public DateTime? ReleasedAt { get; set; } // Aracin gorevden ayrildigi zamani tasir.
    public bool IsActive { get; set; } // Atamanin halen aktif olup olmadigini belirler.
    public Guid? TenantId { get; set; } // Arac-gorev atamasini kiraci sinirinda tutar.

    protected VehicleTask() { }
    public VehicleTask(Guid id) : base(id) { }
}
