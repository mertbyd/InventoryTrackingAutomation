using System;
using InventoryTrackingAutomation.Enums.Workflows;

namespace InventoryTrackingAutomation.Dtos.Workflows;

//işlevi: WorkflowInstanceStep verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class WorkflowInstanceStepDto
{
    /// <summary>
    /// Id alanı.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// WorkflowInstanceId alanı.
    /// </summary>
    public Guid WorkflowInstanceId { get; set; }
    /// <summary>
    /// WorkflowStepDefinitionId alanı.
    /// </summary>
    public Guid WorkflowStepDefinitionId { get; set; }
    /// <summary>
    /// AssignedUserId alanı.
    /// </summary>
    public Guid? AssignedUserId { get; set; }
    /// <summary>
    /// ActionTaken alanı.
    /// </summary>
    public WorkflowActionType ActionTaken { get; set; }
    /// <summary>
    /// Note alanı.
    /// </summary>
    public string? Note { get; set; }
    /// <summary>
    /// ActionDate alanı.
    /// </summary>
    public DateTime? ActionDate { get; set; }
}
