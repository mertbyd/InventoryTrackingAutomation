using InventoryTrackingAutomation.Entities.Masters;
using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Tasks;

/// <summary>
/// Bir envanter gorevinin talep ettigi urun kalemini temsil eden child entity.
/// </summary>
// islevi: InventoryTask icindeki urun ve talep miktari satirini tasir.
// sistemdeki gorevi: VehicleTaskLine tahsislerinin kapasite kaynagi olan operasyonel child kayittir.
public class TaskLine : AuditedEntity<Guid>
{
    public Guid TaskId { get; set; }        // Bagli oldugu envanter gorevi baglami.
    public Guid ProductId { get; set; }     // Talep edilen urun baglami.
    public int Quantity { get; set; }       // Gorev icin talep edilen toplam miktar.

    public virtual InventoryTask Task { get; set; }
    public virtual Product Product { get; set; }
    protected TaskLine() { }
    public TaskLine(Guid id) : base(id) { }
}


