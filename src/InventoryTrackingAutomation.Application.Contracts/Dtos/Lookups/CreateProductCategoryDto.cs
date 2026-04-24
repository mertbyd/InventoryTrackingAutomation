using System;

namespace InventoryTrackingAutomation.Dtos.Lookups;

/// <summary>
/// Ürün kategorisi oluşturma request DTO'su.
/// </summary>
public class CreateProductCategoryDto
{
    public string Code { get; set; }       // Kategori kodu. Örnek: "CAT-001"
    public string Name { get; set; }       // Kategori adı. Örnek: "Elektrik Malzemeleri"
    public Guid? ParentId { get; set; }    // Üst kategori kimliği. Örnek: Ana Kategori Id'si
}
