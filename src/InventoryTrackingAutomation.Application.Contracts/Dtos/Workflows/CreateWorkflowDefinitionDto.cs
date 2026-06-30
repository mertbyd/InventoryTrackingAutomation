using System.Collections.Generic;

namespace InventoryTrackingAutomation.Dtos.Workflows;

//işlevi: CreateWorkflowDefinition verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class CreateWorkflowDefinitionDto
{
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// IsActive alanı.
    /// </summary>
    public bool IsActive { get; set; }
    /// <summary>
    /// Version alanı.
    /// </summary>
    public int Version { get; set; }

    public List<CreateWorkflowStepDefinitionDto> Steps { get; set; } = new();
}
