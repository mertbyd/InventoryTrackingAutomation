using System;

namespace InventoryTrackingAutomation.Dtos.Workflows;

//işlevi: WorkflowStepDefinition verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class WorkflowStepDefinitionDto
{
    /// <summary>
    /// Id alanı.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// StepOrder alanı.
    /// </summary>
    public int StepOrder { get; set; }
    /// <summary>
    /// RequiredRoleName alanı.
    /// </summary>
    public string? RequiredRoleName { get; set; }
    /// <summary>
    /// ResolverKey alanı.
    /// </summary>
    public string? ResolverKey { get; set; }
}
