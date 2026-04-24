using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Shipments;

/// <summary>
/// Sevkiyatın satır kalemlerini (hangi hareket talebi satırından kaç adet) temsil eden child entity.
/// </summary>
public class ShipmentLine : FullAuditedEntity<Guid>
{
    public Guid ShipmentId { get; set; }                // Bağlı olduğu sevkiyat kimliği. Örnek: Shipment Id'si
    public Guid MovementRequestLineId { get; set; }     // Bağlı olduğu hareket talebi satırı kimliği. Örnek: MovementRequestLine Id'si
    public Guid ProductId { get; set; }                 // Sevk edilen ürün kimliği. Örnek: Product Id'si
    public int Quantity { get; set; }                   // Sevk edilen ürün miktarı. Örnek: 20

    protected ShipmentLine() { }
    public ShipmentLine(Guid id) : base(id) { }
}
