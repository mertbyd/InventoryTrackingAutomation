using System;
using System.Collections.Generic;

namespace InventoryTrackingAutomation.Dtos.Workflows;

/// <summary>
/// İş akışı tanımını dönen DTO.
/// </summary>
public class WorkflowDefinitionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int Version { get; set; }
    
    public List<WorkflowStepDefinitionDto> Steps { get; set; } = new();
}
