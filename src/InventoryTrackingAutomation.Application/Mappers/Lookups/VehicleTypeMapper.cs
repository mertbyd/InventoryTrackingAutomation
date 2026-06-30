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
public partial class VehicleTypeMapper
{
    public partial VehicleTypeDto MapToDto(VehicleType entity);
    public partial List<VehicleTypeDto> MapToDto(List<VehicleType> entities);
    public partial CreateVehicleTypeModel MapToModel(CreateVehicleTypeDto dto);
    public partial UpdateVehicleTypeModel MapToModel(UpdateVehicleTypeDto dto);
    public partial void MapToEntity(CreateVehicleTypeModel source, [MappingTarget] VehicleType target);
    public partial void MapToEntity(UpdateVehicleTypeModel source, [MappingTarget] VehicleType target);
}