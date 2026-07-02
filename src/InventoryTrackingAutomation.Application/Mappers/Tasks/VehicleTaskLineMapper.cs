using System.Collections.Generic;
using Riok.Mapperly.Abstractions;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;
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

namespace InventoryTrackingAutomation.Application.Mappers.Tasks;

[Mapper]
public partial class VehicleTaskLineMapper
{
    // VehicleTaskLine.TaskLine (nested) -> TaskLineDto eslemesini devreder; urune buradan ulasilir.
    [UseMapper]
    private readonly TaskLineMapper _taskLineMapper = new();

    public partial CreateVehicleTaskLineModel MapToModel(CreateVehicleTaskLineDto source);
    public partial UpdateVehicleTaskLineModel MapToModel(UpdateVehicleTaskLineDto source);
    public partial VehicleTaskLineDto MapToDto(VehicleTaskLine source);
    public partial void MapToEntity(CreateVehicleTaskLineModel source, [MappingTarget] VehicleTaskLine target);
    public partial void MapToEntity(UpdateVehicleTaskLineModel source, [MappingTarget] VehicleTaskLine target);
}