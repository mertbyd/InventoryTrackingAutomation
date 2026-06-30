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
public partial class WarehouseMapper
{
    public partial WarehouseDto MapToDto(Warehouse source);
    public partial List<WarehouseDto> MapToDto(List<Warehouse> source);
    public partial CreateWarehouseModel MapToModel(CreateWarehouseDto source);
    public partial UpdateWarehouseModel MapToModel(UpdateWarehouseDto source);
    public partial void MapToEntity(CreateWarehouseModel source, [MappingTarget] Warehouse target);
    public partial void MapToEntity(UpdateWarehouseModel source, [MappingTarget] Warehouse target);
}