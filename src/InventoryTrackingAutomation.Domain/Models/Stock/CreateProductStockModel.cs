using System;

namespace InventoryTrackingAutomation.Models.Stock;

/// <summary>
/// Ürün stok kaydı oluşturma domain modeli — Service'ten Manager'a taşınan veri taşıyıcı.
/// </summary>
public class CreateProductStockModel
{
    public Guid ProductId { get; set; }         // Ürün kimliği. Örnek: Product Id'si
    public Guid SiteId { get; set; }            // Lokasyon kimliği. Örnek: Site Id'si
    public int TotalQuantity { get; set; }      // Toplam fiziksel stok. Örnek: 500
    public int ReservedQuantity { get; set; }   // Rezerve stok. Örnek: 50
}
