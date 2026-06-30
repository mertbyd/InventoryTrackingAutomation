using System.Collections.Generic;
using Riok.Mapperly.Abstractions;
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
using InventoryTrackingAutomation.Dtos.Workflows;
using InventoryTrackingAutomation.Models.Workflows;
using InventoryTrackingAutomation.Entities.Workflows;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using InventoryTrackingAutomation.Entities.Lookups;

namespace InventoryTrackingAutomation.Application.Mappers.Workflows;

[Mapper]
public partial class WorkflowMapper
{
    public partial StartWorkflowModel MapToModel(StartWorkflowDto source);
    public partial WorkflowInstanceDto MapToDto(WorkflowInstance source);
    public partial ProcessApprovalModel MapToModel(ProcessApprovalDto source);
    public partial WorkflowInstanceStepDto MapToDto(WorkflowInstanceStep source);
}