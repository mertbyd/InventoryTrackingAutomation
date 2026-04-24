using System;

namespace InventoryTrackingAutomation.Dtos.Movements;

/// <summary>
/// Hareket talebi satırı güncelleme request DTO'su.
/// </summary>
public class UpdateMovementRequestLineDto
{
    public Guid MovementRequestId { get; set; }     // Bağlı talep Id.
    public Guid ProductId { get; set; }             // Ürün Id.
    public int Quantity { get; set; }               // Talep edilen miktar. Örnek: 20
}
