using System;

namespace InventoryTrackingAutomation.Dtos.Workflows;

/// <summary>
/// İş akışı adımı tanımını dönen DTO.
/// </summary>
public class WorkflowStepDefinitionDto
{
    public Guid Id { get; set; }
    public int StepOrder { get; set; }
    public string? RequiredRoleName { get; set; }
    public bool IsManagerApprovalRequired { get; set; }
}
