using System.Collections.Generic;

namespace InventoryTrackingAutomation.Dtos.Workflows;

/// <summary>
/// İş akışı tanımı oluşturmak için kullanılan DTO.
/// </summary>
public class CreateWorkflowDefinitionDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int Version { get; set; }
    
    public List<CreateWorkflowStepDefinitionDto> Steps { get; set; } = new();
}
