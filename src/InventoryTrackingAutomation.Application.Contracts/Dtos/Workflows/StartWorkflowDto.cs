using System;

namespace InventoryTrackingAutomation.Dtos.Workflows;

//işlevi: StartWorkflow verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class StartWorkflowDto
{
    public string EntityType { get; set; } = string.Empty;
    /// <summary>
    /// EntityId alanı.
    /// </summary>
    public Guid EntityId { get; set; }
    /// <summary>
    /// WorkflowDefinitionId alanı.
    /// </summary>
    public Guid WorkflowDefinitionId { get; set; }
}
