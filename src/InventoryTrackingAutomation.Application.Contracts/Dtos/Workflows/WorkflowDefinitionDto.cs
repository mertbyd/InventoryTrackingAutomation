using System;
using System.Collections.Generic;

namespace InventoryTrackingAutomation.Dtos.Workflows;

/// <summary>
/// İş akışı tanımını dönen DTO.
/// </summary>
//işlevi: WorkflowDefinition verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class WorkflowDefinitionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int Version { get; set; }
    
    public List<WorkflowStepDefinitionDto> Steps { get; set; } = new();
}
