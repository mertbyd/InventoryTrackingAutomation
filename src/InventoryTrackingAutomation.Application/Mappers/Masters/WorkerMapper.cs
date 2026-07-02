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
public partial class WorkerMapper
{
    // Worker entity'sinde isim alanı yoktur; yönetici display'i sicil numarasından gelir.
    [MapProperty(nameof(Worker.Manager) + "." + nameof(Worker.RegistrationNumber), nameof(WorkerDto.ManagerName))]
    public partial WorkerDto MapToDto(Worker source);
    public partial List<WorkerDto> MapToDto(List<Worker> source);
    public partial CreateWorkerModel MapToModel(CreateWorkerDto source);
    public partial UpdateWorkerModel MapToModel(UpdateWorkerDto source);
    public partial void MapToEntity(CreateWorkerModel source, [MappingTarget] Worker target);
    public partial void MapToEntity(UpdateWorkerModel source, [MappingTarget] Worker target);
}