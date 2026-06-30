using System.Collections.Generic;
using Riok.Mapperly.Abstractions;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Models.Masters;
using InventoryTrackingAutomation.Entities.Masters;
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

namespace InventoryTrackingAutomation.Application.Mappers.Masters;

[Mapper]
public partial class ProductMapper
{
    public partial ProductDto MapToDto(Product source);
    public partial List<ProductDto> MapToDto(List<Product> source);
    public partial ProductStockSummaryDto MapToDto(ProductStockSummaryModel source);
    public partial CreateProductModel MapToModel(CreateProductDto source);
    public partial UpdateProductModel MapToModel(UpdateProductDto source);
    public partial void MapToEntity(CreateProductModel source, [MappingTarget] Product target);
    public partial void MapToEntity(UpdateProductModel source, [MappingTarget] Product target);
}