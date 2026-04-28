using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Tasks;

/// <summary>
/// Bir aracin belirli bir envanter gorevine atanmasini temsil eder.
/// </summary>
public class VehicleTask : FullAuditedEntity<Guid>
{
    public Guid VehicleId { get; set; }       // Goreve atanan arac Id'si.
    public Guid InventoryTaskId { get; set; } // Aracin bagli oldugu gorev Id'si.
    public DateTime AssignedAt { get; set; }  // Aracin goreve atandigi zaman.
    public DateTime? ReleasedAt { get; set; } // Aracin gorevden ayrildigi zaman.
    public bool IsActive { get; set; }        // Atama aktif mi.

    protected VehicleTask() { }
    public VehicleTask(Guid id) : base(id) { }
}
