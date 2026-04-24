using System;
using Volo.Abp.Application.Dtos;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Stock;

/// <summary>
/// Stok hareketi response DTO'su — GetAll ve GetById operasyonlarında döner.
/// </summary>
public class StockMovementDto : FullAuditedEntityDto<Guid>
{
    public Guid ProductId { get; set; }                     // Ürün kimliği.
    public Guid SiteId { get; set; }                        // Lokasyon kimliği.
    public StockMovementTypeEnum MovementType { get; set; } // Hareket tipi. Örnek: StockMovementTypeEnum.In
    public int Quantity { get; set; }                       // Hareket miktarı. Örnek: 100
    public int BalanceAfter { get; set; }                   // Hareketten sonraki bakiye. Örnek: 600
    public int ReservedAfter { get; set; }                  // Hareketten sonraki rezerve. Örnek: 50
    public string ReferenceType { get; set; }               // Kaynak tipi. Örnek: "MovementRequest"
    public Guid? ReferenceId { get; set; }                  // Kaynak kimliği.
    public string? Note { get; set; }                       // Açıklama notu.
    public DateTime OccurredAt { get; set; }                // Hareket zamanı.
}
