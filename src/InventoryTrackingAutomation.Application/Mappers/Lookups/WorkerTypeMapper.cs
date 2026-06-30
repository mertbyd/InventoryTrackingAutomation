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
public partial class WorkerTypeMapper
{
    public partial WorkerTypeDto MapToDto(WorkerType entity);
    public partial List<WorkerTypeDto> MapToDto(List<WorkerType> entities);
    public partial CreateWorkerTypeModel MapToModel(CreateWorkerTypeDto dto);
    public partial UpdateWorkerTypeModel MapToModel(UpdateWorkerTypeDto dto);
    public partial void MapToEntity(CreateWorkerTypeModel source, [MappingTarget] WorkerType target);
    public partial void MapToEntity(UpdateWorkerTypeModel source, [MappingTarget] WorkerType target);
}