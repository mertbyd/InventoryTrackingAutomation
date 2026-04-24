using System;

namespace InventoryTrackingAutomation.Models.Movements;

/// <summary>
/// Hareket talebi satırı güncelleme domain modeli — Service'ten Manager'a taşınan veri taşıyıcı.
/// </summary>
public class UpdateMovementRequestLineModel
{
    public Guid MovementRequestId { get; set; }     // Bağlı talep Id. Örnek: MovementRequest Id'si
    public Guid ProductId { get; set; }             // Ürün Id. Örnek: Product Id'si
    public int Quantity { get; set; }               // Talep edilen miktar. Örnek: 20
}
