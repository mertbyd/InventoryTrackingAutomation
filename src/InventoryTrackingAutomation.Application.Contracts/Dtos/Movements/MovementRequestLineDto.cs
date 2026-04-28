using System;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Movements;

/// <summary>
/// Hareket talebi satırı response DTO'su — GetAll ve GetById operasyonlarında döner.
/// </summary>
//işlevi: MovementRequestLine verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class MovementRequestLineDto : FullAuditedEntityDto<Guid>
{
    public Guid MovementRequestId { get; set; }     // Bağlı talep Id.
    public Guid ProductId { get; set; }             // Ürün Id.
    public int Quantity { get; set; }               // Talep edilen miktar. Örnek: 20
}
