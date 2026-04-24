using System;

namespace InventoryTrackingAutomation.Dtos.Stock;

/// <summary>
/// Ürün stok kaydı oluşturma request DTO'su.
/// </summary>
public class CreateProductStockDto
{
    public Guid ProductId { get; set; }         // Ürün kimliği.
    public Guid SiteId { get; set; }            // Lokasyon kimliği.
    public int TotalQuantity { get; set; }      // Toplam fiziksel stok. Örnek: 500
    public int ReservedQuantity { get; set; }   // Rezerve stok. Örnek: 50
}
