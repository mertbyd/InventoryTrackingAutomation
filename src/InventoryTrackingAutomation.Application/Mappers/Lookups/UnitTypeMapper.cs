using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using InventoryTrackingAutomation.Entities.Lookups;
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
public partial class UnitTypeMapper
{
    public partial UnitTypeDto MapToDto(UnitType entity);
    public partial List<UnitTypeDto> MapToDto(List<UnitType> entities);
    public partial CreateUnitTypeModel MapToModel(CreateUnitTypeDto dto);
    public partial UpdateUnitTypeModel MapToModel(UpdateUnitTypeDto dto);
    public partial void MapToEntity(CreateUnitTypeModel source, [MappingTarget] UnitType target);
    public partial void MapToEntity(UpdateUnitTypeModel source, [MappingTarget] UnitType target);
}