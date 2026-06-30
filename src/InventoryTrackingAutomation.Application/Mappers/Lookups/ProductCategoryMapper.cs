using System.Collections.Generic;
using Riok.Mapperly.Abstractions;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Dtos.Inventory;
using InventoryTrackingAutomation.Models.Inventory;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Models.Masters;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Models.Movements;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using InventoryTrackingAutomation.Entities.Lookups;

namespace InventoryTrackingAutomation.Application.Mappers.Lookups;

[Mapper]
public partial class ProductCategoryMapper
{
    public partial ProductCategoryDto MapToDto(ProductCategory source);
    public partial List<ProductCategoryDto> MapToDto(List<ProductCategory> source);
    public partial CreateProductCategoryModel MapToModel(CreateProductCategoryDto source);
    public partial UpdateProductCategoryModel MapToModel(UpdateProductCategoryDto source);
    public partial void MapToEntity(CreateProductCategoryModel source, [MappingTarget] ProductCategory target);
    public partial void MapToEntity(UpdateProductCategoryModel source, [MappingTarget] ProductCategory target);
}