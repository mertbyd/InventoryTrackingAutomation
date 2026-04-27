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

    /// <summary>
    /// Onaycı çözümleme anahtarı. Örnekler: "InitiatorManager", "SourceSiteManager", "TargetSiteManager".
    /// Null/boş ise sadece RequiredRoleName temelli rol bazlı yetki kontrolü yapılır.
    /// </summary>
    public string? ResolverKey { get; set; }
}
