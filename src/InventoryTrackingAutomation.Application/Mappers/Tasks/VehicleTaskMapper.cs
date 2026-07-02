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
public partial class VehicleTaskMapper
{
    // VehicleTaskLine -> VehicleTaskLineDto eslemesini (ProductId/ProductName dahil) tekrar etmemek icin devreder.
    [UseMapper]
    private readonly VehicleTaskLineMapper _lineMapper = new();

    public partial List<VehicleTaskDto> MapToDto(List<VehicleTask> source);
    public partial CreateVehicleTaskModel MapToModel(CreateVehicleTaskDto source);
    public partial List<CreateVehicleTaskLineModel> MapToModel(List<CreateVehicleTaskLineDto> source);
    public partial UpdateVehicleTaskModel MapToModel(UpdateVehicleTaskDto source);
    [MapProperty(nameof(VehicleTask.VehicleTaskLines), nameof(VehicleTaskDto.Lines))]
    public partial VehicleTaskDto MapToDto(VehicleTask source);
    public partial void MapToEntity(CreateVehicleTaskModel source, [MappingTarget] VehicleTask target);
    public partial void MapToEntity(UpdateVehicleTaskModel source, [MappingTarget] VehicleTask target);
}