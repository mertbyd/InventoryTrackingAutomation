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
public partial class InventoryTaskMapper
{
    public partial List<InventoryTaskDto> MapToDto(List<InventoryTask> source);
    public partial List<TaskVehicleDto> MapToDto(List<TaskVehicleModel> source);
    public partial List<TaskInventoryDto> MapToDto(List<TaskInventoryModel> source);
    public partial CreateInventoryTaskModel MapToModel(CreateInventoryTaskDto source);
    public partial List<CreateTaskLineModel> MapToModel(List<CreateTaskLineDto> source);
    public partial UpdateInventoryTaskModel MapToModel(UpdateInventoryTaskDto source);
    public partial InventoryTaskDto MapToDto(InventoryTask source);
    public partial UpdateInventoryTaskModel MapToModel(InventoryTask source);
    public partial void MapToEntity(CreateInventoryTaskModel source, [MappingTarget] InventoryTask target);
    public partial void MapToEntity(UpdateInventoryTaskModel source, [MappingTarget] InventoryTask target);
}