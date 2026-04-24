using System;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Lookups;

/// <summary>
/// Ürün kategorisi response DTO'su — GetAll ve GetById operasyonlarında döner.
/// </summary>
public class ProductCategoryDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; }       // Kategori kodu. Örnek: "CAT-001"
    public string Name { get; set; }       // Kategori adı. Örnek: "Elektrik Malzemeleri"
    public Guid? ParentId { get; set; }    // Üst kategori kimliği. Örnek: Ana Kategori Id'si
}
