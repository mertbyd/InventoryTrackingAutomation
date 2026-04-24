using System;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Stock;

/// <summary>
/// Ürün stok durumu response DTO'su — GetAll ve GetById operasyonlarında döner.
/// </summary>
public class ProductStockDto : FullAuditedEntityDto<Guid>
{
    public Guid ProductId { get; set; }         // Ürün kimliği.
    public Guid SiteId { get; set; }            // Lokasyon kimliği.
    public int TotalQuantity { get; set; }      // Toplam fiziksel stok. Örnek: 500
    public int ReservedQuantity { get; set; }   // Rezerve stok. Örnek: 50
}
