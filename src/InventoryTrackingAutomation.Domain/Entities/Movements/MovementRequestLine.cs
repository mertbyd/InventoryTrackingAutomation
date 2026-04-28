using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Movements;

/// <summary>
/// Hareket talebinin satır kalemlerini (hangi üründen kaç adet) temsil eden child entity.
/// </summary>
public class MovementRequestLine : FullAuditedEntity<Guid>
{
    public Guid MovementRequestId { get; set; }  // Bağlı olduğu talep kimliği. Örnek: MovementRequest Id'si
    public Guid ProductId { get; set; }          // Talep edilen ürün kimliği. Örnek: Product Id'si
    public int Quantity { get; set; }            // Talep edilen ürün miktarı. Örnek: 20

    public int ReceivedQuantity { get; set; }    // Iade tesliminde depoya saglam giren miktar.
    public int DamagedQuantity { get; set; }     // Iade tesliminde hasarli/kirik olarak ayrilan miktar.
    public int LostQuantity { get; set; }        // Iade tesliminde kayip olarak isaretlenen miktar.
    public int ConsumedQuantity { get; set; }    // Gorevde tuketildigi bildirilen miktar.
    public string? ReceiveNote { get; set; }     // Satir bazli teslim alma/kontrol notu.

    protected MovementRequestLine() { }
    public MovementRequestLine(Guid id) : base(id) { }
}
