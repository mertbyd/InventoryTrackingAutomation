using System;
using Volo.Abp.Application.Dtos;
using InventoryTrackingAutomation.Dtos.Common;

namespace InventoryTrackingAutomation.Dtos.Lookups;

//işlevi: ProductCategory verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class ProductCategoryDto : EnrichedEntityDto<Guid>
{
    /// <summary>
    /// Kategori kodu. Örnek: &quot;CAT-001&quot;
    /// </summary>
    public string Code { get; set; }       // Kategori kodu. Örnek: "CAT-001"
    /// <summary>
    /// Kategori adı. Örnek: &quot;Elektrik Malzemeleri&quot;
    /// </summary>
    public string Name { get; set; }       // Kategori adı. Örnek: "Elektrik Malzemeleri"
    /// <summary>
    /// Üst kategori kimliği. Örnek: Ana Kategori Id&apos;si
    /// </summary>
    public Guid? ParentId { get; set; }    // Üst kategori kimliği. Örnek: Ana Kategori Id'si

    public string ParentName { get; set; }
}



