using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Masters;

/// <summary>
/// Ürün güncelleme request DTO'su.
/// </summary>
//işlevi: UpdateProduct verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class UpdateProductDto
{
    public string Code { get; set; }            // Ürün kodu. Örnek: "PRD-001"
    public string Name { get; set; }            // Ürün adı. Örnek: "Vida M8x20"
    public Guid? CategoryId { get; set; }       // Bağlı kategori Id.
    public UnitTypeEnum BaseUnit { get; set; }  // Ölçü birimi. Örnek: UnitTypeEnum.Piece
    public bool IsActive { get; set; }          // Aktif mi. Örnek: true
    public bool IsSerializable { get; set; }    // Seri numaralı mı. Örnek: false
}
