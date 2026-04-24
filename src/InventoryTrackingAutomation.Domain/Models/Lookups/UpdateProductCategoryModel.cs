using System;

namespace InventoryTrackingAutomation.Models.Lookups;

/// <summary>
/// Ürün kategorisi güncelleme domain modeli — Service'ten Manager'a taşınan veri taşıyıcı.
/// </summary>
public class UpdateProductCategoryModel
{
    public string Code { get; set; }        // Kategori kodu. Örnek: "CAT-001"
    public string Name { get; set; }        // Kategori adı. Örnek: "Elektrik Malzemeleri"
    public Guid? ParentId { get; set; }     // Üst kategori kimliği. Örnek: Ana Kategori Id'si
}
