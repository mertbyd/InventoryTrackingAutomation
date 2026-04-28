using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation.Entities.Tasks;

/// <summary>
/// Bir aracin belirli bir envanter gorevine atanmasini temsil eder.
/// </summary>
public class VehicleTask : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid VehicleId { get; set; }       // Goreve atanan arac Id'si.
    public Guid InventoryTaskId { get; set; } // Aracin bagli oldugu gorev Id'si.
    public Guid DriverWorkerId { get; set; }  // Gorev sirasinda araci kullanan worker Id'si.
    public DateTime AssignedAt { get; set; }  // Aracin goreve atandigi zaman.
    public DateTime? ReleasedAt { get; set; } // Aracin gorevden ayrildigi zaman.
    public bool IsActive { get; set; }        // Atama aktif mi.
    public Guid? TenantId { get; set; }       // ABP tenant izolasyonu icin kiraci Id'si.

    protected VehicleTask() { }
    public VehicleTask(Guid id) : base(id) { }
}
