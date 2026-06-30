using System.Collections.Generic;
using Riok.Mapperly.Abstractions;
using InventoryTrackingAutomation.Dtos.Inventory;
using InventoryTrackingAutomation.Models.Inventory;
using InventoryTrackingAutomation.Entities.Inventory;
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

namespace InventoryTrackingAutomation.Application.Mappers.Inventory;

[Mapper]
public partial class StockLocationMapper
{
    public partial StockLocationDto MapToDto(StockLocation source);
    public partial List<StockLocationDto> MapToDto(List<StockLocation> source);
    public partial CreateStockLocationModel MapToModel(CreateStockLocationDto source);
    public partial UpdateStockLocationModel MapToModel(UpdateStockLocationDto source);
    public partial void MapToEntity(CreateStockLocationModel source, [MappingTarget] StockLocation target);
    public partial void MapToEntity(UpdateStockLocationModel source, [MappingTarget] StockLocation target);
}