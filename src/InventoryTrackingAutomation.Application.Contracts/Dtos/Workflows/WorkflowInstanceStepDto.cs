using System;
using InventoryTrackingAutomation.Enums.Workflows;

namespace InventoryTrackingAutomation.Dtos.Workflows;

/// <summary>
/// İş akışı onay adımını dönen DTO.
/// </summary>
//işlevi: WorkflowInstanceStep verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
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
