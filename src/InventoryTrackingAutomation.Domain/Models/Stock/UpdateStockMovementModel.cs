using System;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Models.Stock;

/// <summary>
/// Stok hareketi güncelleme domain modeli — Service'ten Manager'a taşınan veri taşıyıcı.
/// </summary>
public class UpdateStockMovementModel
{
    public Guid ProductId { get; set; }                      // Ürün kimliği. Örnek: Product Id'si
    public Guid SiteId { get; set; }                         // Lokasyon kimliği. Örnek: Site Id'si
    public StockMovementTypeEnum MovementType { get; set; }  // Hareket tipi. Örnek: StockMovementTypeEnum.In
    public int Quantity { get; set; }                        // Hareket miktarı. Örnek: 100
    public int BalanceAfter { get; set; }                    // Hareketten sonraki bakiye. Örnek: 600
    public int ReservedAfter { get; set; }                   // Hareketten sonraki rezerve. Örnek: 50
    public string ReferenceType { get; set; }                // Kaynak tipi. Örnek: "MovementRequest"
    public Guid? ReferenceId { get; set; }                   // Kaynak kimliği. Örnek: MovementRequest Id'si
    public string? Note { get; set; }                        // Açıklama notu. Örnek: "Manuel düzeltme"
    public DateTime OccurredAt { get; set; }                 // Hareket zamanı. Örnek: DateTime.UtcNow
}
