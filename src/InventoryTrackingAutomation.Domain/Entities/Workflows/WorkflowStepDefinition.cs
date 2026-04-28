using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Workflows;

/// <summary>
/// İş akışı şablonundaki bir adımı temsil eden entity.
/// Onaycı çözümleme tamamen ResolverKey üzerinden yapılır (configuration-based).
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
    /// ResolverKey boşsa rol bazlı yetkilendirme yapılır.
    /// </summary>
    public string? RequiredRoleName { get; private set; }

    /// <summary>
    /// Onaycı çözümleme mantığının anahtarı.
    /// Örnekler: "InitiatorManager", "SourceWarehouseManager", "TargetWarehouseManager".
    /// DefaultWorkflowApproverResolver bu anahtara bakarak dinamik onaycıyı bulur.
    /// Null/boş ise sadece RequiredRoleName ve rol bazlı yetki kontrol edilir.
    /// </summary>
    public string? ResolverKey { get; private set; }

    /// <summary>
    /// Bağlı olduğu iş akışı tanımı navigation property'si.
    /// </summary>
    public virtual WorkflowDefinition WorkflowDefinition { get; private set; }

    private WorkflowStepDefinition()
    {
    }

    public WorkflowStepDefinition(Guid id, Guid workflowDefinitionId, int stepOrder, string? requiredRoleName, string? resolverKey = null)
        : base(id)
    {
        WorkflowDefinitionId = workflowDefinitionId;
        StepOrder = stepOrder;
        RequiredRoleName = requiredRoleName;
        ResolverKey = resolverKey;
    }
}
