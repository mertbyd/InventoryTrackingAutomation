using System;
using System.Collections.Generic;

namespace InventoryTrackingAutomation.Dtos.Workflows;

//işlevi: WorkflowDefinition verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class WorkflowDefinitionDto
{
    /// <summary>
    /// Id alanı.
    /// </summary>
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// IsActive alanı.
    /// </summary>
    public bool IsActive { get; set; }
    /// <summary>
    /// Version alanı.
    /// </summary>
    public int Version { get; set; }
    
    public List<WorkflowStepDefinitionDto> Steps { get; set; } = new();
}
