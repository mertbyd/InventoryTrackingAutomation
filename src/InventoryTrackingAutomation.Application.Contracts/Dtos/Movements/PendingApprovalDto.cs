using System;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using System;

namespace InventoryTrackingAutomation.Dtos.Movements;

// Shows pending approvals assigned to current user. Lists movement requests waiting for this user's decision.
//işlevi: PendingApproval verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class PendingApprovalDto
{
    /// <summary>
    /// MovementRequestId alanı.
    /// </summary>
    public Guid MovementRequestId { get; set; }
    /// <summary>
    /// WorkflowInstanceStepId alanı.
    /// </summary>
    public Guid WorkflowInstanceStepId { get; set; }
    /// <summary>
    /// RequestNumber alanı.
    /// </summary>
    public string RequestNumber { get; set; }
    /// <summary>
    /// SourceWarehouseName alanı.
    /// </summary>
    public string SourceWarehouseName { get; set; }
    /// <summary>
    /// TargetWarehouseName alanı.
    /// </summary>
    public string TargetWarehouseName { get; set; }
    /// <summary>
    /// CurrentStepOrder alanı.
    /// </summary>
    public int CurrentStepOrder { get; set; }
    /// <summary>
    /// CurrentStepName alanı.
    /// </summary>
    public string CurrentStepName { get; set; }
    /// <summary>
    /// CreatedAt alanı.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// PlannedDate alanı.
    /// </summary>
    public DateTime PlannedDate { get; set; }
    /// <summary>
    /// RequestNote alanı.
    /// </summary>
    public string RequestNote { get; set; }
    /// <summary>
    /// Priority alanı.
    /// </summary>
    public MovementPriorityEnum Priority { get; set; }
}
