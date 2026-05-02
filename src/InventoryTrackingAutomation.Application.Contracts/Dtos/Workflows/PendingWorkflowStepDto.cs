using System;
using InventoryTrackingAutomation.Enums.Workflows;

namespace InventoryTrackingAutomation.Dtos.Workflows;

//işlevi: PendingWorkflowStep verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class PendingWorkflowStepDto
{
    /// <summary>
    /// WorkflowInstanceStepId alanı.
    /// </summary>
    public Guid WorkflowInstanceStepId { get; set; }
    /// <summary>
    /// WorkflowInstanceId alanı.
    /// </summary>
    public Guid WorkflowInstanceId { get; set; }
    /// <summary>
    /// EntityType alanı.
    /// </summary>
    public string EntityType { get; set; }
    /// <summary>
    /// EntityId alanı.
    /// </summary>
    public Guid EntityId { get; set; }
    /// <summary>
    /// StepOrder alanı.
    /// </summary>
    public int StepOrder { get; set; }
    /// <summary>
    /// StepName alanı.
    /// </summary>
    public string StepName { get; set; }
    /// <summary>
    /// InitiatorUserId alanı.
    /// </summary>
    public Guid InitiatorUserId { get; set; }
    /// <summary>
    /// CreatedAt alanı.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
