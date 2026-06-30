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
public partial class TaskLineMapper
{
    public partial CreateTaskLineModel MapToModel(CreateTaskLineDto source);
    public partial UpdateTaskLineModel MapToModel(UpdateTaskLineDto source);
    public partial TaskLineDto MapToDto(TaskLine source);
    public partial List<TaskLineDto> MapToDto(List<TaskLine> source);
    public partial void MapToEntity(CreateTaskLineModel source, [MappingTarget] TaskLine target);
    public partial void MapToEntity(UpdateTaskLineModel source, [MappingTarget] TaskLine target);
}