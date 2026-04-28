using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Movements;

// Shows pending approvals assigned to current user. Lists movement requests waiting for this user's decision.
public class PendingApprovalDto
{
    public Guid MovementRequestId { get; set; }
    public Guid WorkflowInstanceStepId { get; set; }
    public string RequestNumber { get; set; }
    public string SourceWarehouseName { get; set; }
    public string TargetWarehouseName { get; set; }
    public int CurrentStepOrder { get; set; }
    public string CurrentStepName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime PlannedDate { get; set; }
    public string RequestNote { get; set; }
    public MovementPriorityEnum Priority { get; set; }
}
