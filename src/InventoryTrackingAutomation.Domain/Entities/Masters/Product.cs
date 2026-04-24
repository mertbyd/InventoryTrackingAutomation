using System;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Masters;

/// <summary>
/// Sistemdeki ürünleri temsil eden master data entity'si.
/// </summary>
public class Product : FullAuditedEntity<Guid>
{
    public string Code { get; set; }            // Ürünün benzersiz kodu. Örnek: "PRD-001"
    public string Name { get; set; }            // Ürünün adı. Örnek: "Vida M8x20"
    public Guid? CategoryId { get; set; }       // Bağlı olduğu ürün kategorisi. Örnek: ProductCategory Id'si
    public UnitTypeEnum BaseUnit { get; set; }  // Temel ölçü birimi. Örnek: UnitTypeEnum.Piece
    public bool IsActive { get; set; }          // Ürün aktif mi. Örnek: true
    public bool IsSerializable { get; set; }    // Seri numarası takibi var mı. Örnek: false

    protected Product() { }
    public Product(Guid id) : base(id) { }
}
