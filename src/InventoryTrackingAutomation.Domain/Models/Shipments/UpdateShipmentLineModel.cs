using System;

namespace InventoryTrackingAutomation.Models.Shipments;

/// <summary>
/// Sevkiyat satırı güncelleme domain modeli — Service'ten Manager'a taşınan veri taşıyıcı.
/// </summary>
public class UpdateShipmentLineModel
{
    public Guid ShipmentId { get; set; }                    // Sevkiyat Id. Örnek: Shipment Id'si
    public Guid MovementRequestLineId { get; set; }         // Bağlı talep satırı Id. Örnek: MovementRequestLine Id'si
    public Guid ProductId { get; set; }                     // Ürün Id. Örnek: Product Id'si
    public int Quantity { get; set; }                       // Sevk edilen ürün miktarı. Örnek: 20
    public string? DamageNote { get; set; }                 // Hasar notu. Örnek: "2 adet ambalaj hasarlı"
}
