using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Models.Masters;

/// <summary>
/// Ürün oluşturma domain modeli — Service'ten Manager'a taşınan veri taşıyıcı.
/// </summary>
public class CreateProductModel
{
    public string Code { get; set; }                // Ürün kodu. Örnek: "PRD-001"
    public string Name { get; set; }                // Ürün adı. Örnek: "Vida M8x20"
    public Guid? CategoryId { get; set; }           // Bağlı kategori Id. Örnek: Guid
    public UnitTypeEnum BaseUnit { get; set; }      // Ölçü birimi. Örnek: UnitTypeEnum.Piece
    public bool IsActive { get; set; }              // Aktif mi. Örnek: true
    public bool IsSerializable { get; set; }        // Seri numaralı mı. Örnek: false
    public int? MinimumStockLevel { get; set; }     // Minimum stok seviyesi. Örnek: 100
}
