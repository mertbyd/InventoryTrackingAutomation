using InventoryTrackingAutomation.Entities.Masters;
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation.Entities.Tasks;

/// <summary>
/// Bir aracin belirli bir envanter gorevine atanma gecmisini temsil eden aggregate.
/// </summary>
// islevi: Aracin bir InventoryTask uzerindeki atama ve serbest birakma zamanini tasir.
// sistemdeki gorevi: VehicleTaskLine tahsislerinin ust operasyonel baglamidir.
public class VehicleTask : AuditedEntity<Guid>
{
    public Guid VehicleId { get; set; } // Operasyona atanan arac baglamini tasir.
    public Guid TaskId { get; set; } // Aracin bagli oldugu operasyon isi baglamini tasir.
    public Guid ResponsibleWorkerId { get; set; } // Arac atamasindan sorumlu calisan baglamini tasir.
    public DateTime AssignedAt { get; set; } // Aracin operasyona dahil edildigi zamani tasir.
    public DateTime? ReleasedAt { get; set; } // Aracin operasyondan ayrildigi zamani tasir.

    public virtual InventoryTask Task { get; protected set; }
    public virtual Vehicle Vehicle { get; protected set; }
    public virtual Worker ResponsibleWorker { get; protected set; }

    protected VehicleTask() { }
    public VehicleTask(Guid id) : base(id) { }
}


