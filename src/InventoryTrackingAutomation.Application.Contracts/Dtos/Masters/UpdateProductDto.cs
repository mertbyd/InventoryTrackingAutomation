using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Masters;

//işlevi: UpdateProduct verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class UpdateProductDto
{
    /// <summary>
    /// Ürün kodu. Örnek: &quot;PRD-001&quot;
    /// </summary>
    public string Code { get; set; }            // Ürün kodu. Örnek: "PRD-001"
    /// <summary>
    /// Ürün adı. Örnek: &quot;Vida M8x20&quot;
    /// </summary>
    public string Name { get; set; }            // Ürün adı. Örnek: "Vida M8x20"
    /// <summary>
    /// Bağlı kategori Id.
    /// </summary>
    public Guid? CategoryId { get; set; }       // Bağlı kategori Id.
    /// <summary>
    /// Ölçü birimi. Örnek: UnitTypeEnum.Piece
    /// </summary>
    public UnitTypeEnum BaseUnit { get; set; }  // Ölçü birimi. Örnek: UnitTypeEnum.Piece
    /// <summary>
    /// Aktif mi. Örnek: true
    /// </summary>
    public bool IsActive { get; set; }          // Aktif mi. Örnek: true
    /// <summary>
    /// Seri numaralı mı. Örnek: false
    /// </summary>
    public bool IsSerializable { get; set; }    // Seri numaralı mı. Örnek: false
}
