using System.Collections.Generic;
using Riok.Mapperly.Abstractions;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Models.Movements;
using InventoryTrackingAutomation.Entities.Movements;
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

namespace InventoryTrackingAutomation.Application.Mappers.Movements;

[Mapper]
public partial class MovementRequestMapper
{
    public partial CreateMovementRequestModel MapToModel(CreateMovementRequestDto source);
    public partial UpdateMovementRequestModel MapToModel(UpdateMovementRequestDto source);
    public partial ReceiveMovementRequestModel MapToModel(ReceiveMovementRequestDto source);
    public partial MovementRequestDto MapToDto(MovementRequest source);
    public partial void MapToEntity(CreateMovementRequestModel source, [MappingTarget] MovementRequest target);
    public partial void MapToEntity(UpdateMovementRequestModel source, [MappingTarget] MovementRequest target);
}