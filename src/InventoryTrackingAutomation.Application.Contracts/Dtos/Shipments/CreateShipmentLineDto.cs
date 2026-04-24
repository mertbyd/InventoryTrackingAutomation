using System;

namespace InventoryTrackingAutomation.Dtos.Shipments;

/// <summary>
/// Sevkiyat satırı oluşturma request DTO'su.
/// </summary>
public class CreateShipmentLineDto
{
    public Guid ShipmentId { get; set; }                    // Sevkiyat Id.
    public Guid MovementRequestLineId { get; set; }         // Bağlı talep satırı Id.
    public Guid ProductId { get; set; }                     // Ürün Id.
    public int Quantity { get; set; }                       // Sevk edilen miktar. Örnek: 20
}
