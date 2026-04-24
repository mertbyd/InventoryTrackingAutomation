using System;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Shipments;

/// <summary>
/// Sevkiyat satırı response DTO'su — GetAll ve GetById operasyonlarında döner.
/// </summary>
public class ShipmentLineDto : FullAuditedEntityDto<Guid>
{
    public Guid ShipmentId { get; set; }                    // Sevkiyat Id.
    public Guid MovementRequestLineId { get; set; }         // Bağlı talep satırı Id.
    public Guid ProductId { get; set; }                     // Ürün Id.
    public int Quantity { get; set; }                       // Sevk edilen miktar. Örnek: 20
}
