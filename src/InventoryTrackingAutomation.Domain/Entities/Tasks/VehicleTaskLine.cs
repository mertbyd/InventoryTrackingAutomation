using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Tasks;

/// <summary>
/// Bir arac-gorev atamasindaki urun tahsisini ve iade uzlasmasini temsil eden child entity.
/// </summary>
// islevi: Arac-gorev atamasindaki urun tahsisini ve iade teslim miktarlarini tasir.
// sistemdeki gorevi: TaskLine talebinin hangi araca ne kadar tahsis edildigini ve iade uzlasmasini belirler.
public class VehicleTaskLine : AuditedEntity<Guid>
{
    public Guid VehicleTaskId { get; set; }     // Bagli oldugu arac-gorev atamasi baglami.
    public Guid TaskLineId { get; set; }        // Kaynak gorev kalemi baglami.
    public int AllocatedQuantity { get; set; }  // Bu araca tahsis edilen miktar.
    public int ReceivedQuantity { get; set; }   // Iade tesliminde depoya saglam giren miktar.
    public int DamagedQuantity { get; set; }    // Iade tesliminde hasarli olarak ayrilan miktar.   
    public int LostQuantity { get; set; }       // Iade tesliminde kayip olarak isaretlenen miktar.
    public int ConsumedQuantity { get; set; }   // Gorevde tuketildigi bildirilen miktar.
    public string? ReceiveNote { get; set; }    // Satir bazli teslim alma notu.

    protected VehicleTaskLine() { }
    public VehicleTaskLine(Guid id) : base(id) { }
}
