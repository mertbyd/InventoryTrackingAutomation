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
public partial class VehicleMapper
{
    public partial VehicleDto MapToDto(Vehicle source);
    public partial List<VehicleDto> MapToDto(List<Vehicle> source);
    public partial List<VehicleInventoryDto> MapToDto(List<VehicleInventoryModel> source);
    public partial CreateVehicleModel MapToModel(CreateVehicleDto source);
    public partial UpdateVehicleModel MapToModel(UpdateVehicleDto source);
    public partial void MapToEntity(CreateVehicleModel source, [MappingTarget] Vehicle target);
    public partial void MapToEntity(UpdateVehicleModel source, [MappingTarget] Vehicle target);
}