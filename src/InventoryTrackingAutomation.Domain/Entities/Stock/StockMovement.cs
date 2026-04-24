using System;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Stock;

/// <summary>
/// Stok üzerinde gerçekleşen her türlü hareketi kayıt altına alan denetim kaydı entity'si.
/// </summary>
public class StockMovement : FullAuditedEntity<Guid>
{
    public Guid ProductId { get; set; }                        // Hareket gerçekleşen ürün kimliği. Örnek: Product Id'si
    public Guid SiteId { get; set; }                           // Hareketin gerçekleştiği lokasyon kimliği. Örnek: Site Id'si
    public StockMovementTypeEnum MovementType { get; set; }   // Hareket tipi. Örnek: StockMovementTypeEnum.In
    public int Quantity { get; set; }                          // Hareket miktarı. Örnek: 100
    public int BalanceAfter { get; set; }                      // Hareketten sonraki toplam stok bakiyesi. Örnek: 600
    public int ReservedAfter { get; set; }                     // Hareketten sonraki rezerve stok miktarı. Örnek: 50
    public string ReferenceType { get; set; }                  // Hareketi tetikleyen kaynak tipi. Örnek: "MovementRequest"
    public Guid? ReferenceId { get; set; }                     // Hareketi tetikleyen kaynak kimliği. Örnek: MovementRequest Id'si
    public string? Note { get; set; }                          // Hareketle ilgili açıklama notu. Örnek: "Manuel düzeltme - sayım farkı"
    public DateTime OccurredAt { get; set; }                   // Hareketin gerçekleştiği zaman. Örnek: DateTime.UtcNow

    protected StockMovement() { }
    public StockMovement(Guid id) : base(id) { }
}
