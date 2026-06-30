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
public partial class DepartmentMapper
{
    public partial DepartmentDto MapToDto(Department source);
    public partial List<DepartmentDto> MapToDto(List<Department> source);
    public partial CreateDepartmentModel MapToModel(CreateDepartmentDto source);
    public partial UpdateDepartmentModel MapToModel(UpdateDepartmentDto source);
    public partial void MapToEntity(CreateDepartmentModel source, [MappingTarget] Department target);
    public partial void MapToEntity(UpdateDepartmentModel source, [MappingTarget] Department target);
}