using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Stock;

/// <summary>
/// Bir ürünün belirli bir lokasyondaki anlık stok durumunu temsil eden entity.
/// </summary>
public class ProductStock : FullAuditedEntity<Guid>
{
    public Guid ProductId { get; set; }       // Stoku takip edilen ürün kimliği. Örnek: Product Id'si
    public Guid SiteId { get; set; }          // Stokun bulunduğu lokasyon kimliği. Örnek: Site Id'si
    public int TotalQuantity { get; set; }    // Toplam fiziksel stok miktarı. Örnek: 500
    public int ReservedQuantity { get; set; } // Rezerve edilmiş (kullanılamaz) stok miktarı. Örnek: 50

    protected ProductStock() { }
    public ProductStock(Guid id) : base(id) { }
}
