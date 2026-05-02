using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Tasks;

/// <summary>
/// Bir envanter gorevinin talep ettigi urun kalemini temsil eden child entity.
/// </summary>
public class TaskLine : FullAuditedEntity<Guid>
{
    public Guid TaskId { get; set; }        // Bagli oldugu envanter gorevi baglami.
    public Guid ProductId { get; set; }     // Talep edilen urun baglami.
    public int Quantity { get; set; }       // Gorev icin talep edilen toplam miktar.

    protected TaskLine() { }
    public TaskLine(Guid id) : base(id) { }
}
