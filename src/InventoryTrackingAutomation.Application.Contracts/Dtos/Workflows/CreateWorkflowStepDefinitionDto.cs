namespace InventoryTrackingAutomation.Dtos.Workflows;

/// <summary>
/// İş akışı adımı tanımı oluşturmak için kullanılan DTO.
/// </summary>
public class CreateWorkflowStepDefinitionDto
{
    public int StepOrder { get; set; }
    public string? RequiredRoleName { get; set; }
    public bool IsManagerApprovalRequired { get; set; }
}
