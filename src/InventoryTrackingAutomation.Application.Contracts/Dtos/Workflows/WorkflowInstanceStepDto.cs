using System;
using InventoryTrackingAutomation.Enums.Workflows;

namespace InventoryTrackingAutomation.Dtos.Workflows;

/// <summary>
/// İş akışı onay adımını dönen DTO.
/// </summary>
public class WorkflowInstanceStepDto
{
    public Guid Id { get; set; }
    public Guid WorkflowInstanceId { get; set; }
    public Guid WorkflowStepDefinitionId { get; set; }
    public Guid? AssignedUserId { get; set; }
    public WorkflowActionType ActionTaken { get; set; }
    public string? Note { get; set; }
    public DateTime? ActionDate { get; set; }
}
