using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Workflows;

/// <summary>
/// İş akışı şablonundaki bir adımı temsil eden entity.
/// </summary>
public class WorkflowStepDefinition : AuditedEntity<Guid>
{
    /// <summary>
    /// Bağlı olduğu iş akışı tanımı ID'si.
    /// </summary>
    public Guid WorkflowDefinitionId { get; private set; }

    /// <summary>
    /// Adımın sırası (1, 2, 3...).
    /// </summary>
    public int StepOrder { get; private set; }

    /// <summary>
    /// Bu adımı onaylayabilecek olan rol adı (Örn: "DepartmentManager").
    /// Eğer IsManagerApprovalRequired true ise null olabilir.
    /// </summary>
    public string? RequiredRoleName { get; private set; }

    /// <summary>
    /// Bu adımda, talebi başlatan kişinin kendi yöneticisinin onayı gerekip gerekmediğini belirtir.
    /// </summary>
    public bool IsManagerApprovalRequired { get; private set; }

    /// <summary>
    /// Özel bir çözümleme mantığı gerekiyorsa buraya anahtar (Örn: "SourceSiteManager") yazılır.
    /// DefaultWorkflowApproverResolver bu anahtara bakarak dinamik onaycıyı bulur.
    /// </summary>
    public string? ResolverKey { get; private set; }

    /// <summary>
    /// Bağlı olduğu iş akışı tanımı navigation property'si.
    /// </summary>
    public virtual WorkflowDefinition WorkflowDefinition { get; private set; }

    private WorkflowStepDefinition()
    {
    }

    public WorkflowStepDefinition(Guid id, Guid workflowDefinitionId, int stepOrder, string? requiredRoleName, bool isManagerApprovalRequired, string? resolverKey = null)
        : base(id)
    {
        WorkflowDefinitionId = workflowDefinitionId;
        StepOrder = stepOrder;
        RequiredRoleName = requiredRoleName;
        IsManagerApprovalRequired = isManagerApprovalRequired;
        ResolverKey = resolverKey;
    }
}
