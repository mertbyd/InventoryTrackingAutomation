using System;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Masters;

//işlevi: Product verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class ProductDto : EntityDto<Guid>
{
    /// <summary>
    /// Ürün kodu. Örnek: &quot;PRD-001&quot;
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Ürün adı. Örnek: &quot;Vida M8x20&quot;
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Bağlı kategori Id.
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Bağlı kategori adı (Category navigation'ından doldurulur).
    /// </summary>
    public string CategoryName { get; set; }

    /// <summary>
    /// Ölçü birimi Id (Lookup FK).
    /// </summary>
    public Guid UnitTypeId { get; set; }

    /// <summary>
    /// Ölçü birimi adı (UnitType navigation'ından doldurulur).
    /// </summary>
    public string UnitTypeName { get; set; }
}
