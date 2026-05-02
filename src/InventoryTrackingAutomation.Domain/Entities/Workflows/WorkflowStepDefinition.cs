using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Workflows;

/// <summary>
/// Is akisi sablonundaki tek bir onay adimini temsil eden entity.
/// </summary>
public class WorkflowStepDefinition : AuditedEntity<Guid>
{
    /// <summary>
    /// Bagli oldugu is akisi taniminin kimligini tasir.
    /// </summary>
    public Guid WorkflowDefinitionId { get; private set; }

    /// <summary>
    /// Adimin is akisi icindeki sirasini belirler.
    /// </summary>
    public int StepOrder { get; private set; }

    /// <summary>
    /// ResolverKey kullanilmadiginda onay icin gereken rol adini tasir.
    /// </summary>
    public string? RequiredRoleName { get; private set; }

    /// <summary>
    /// Dinamik onayci cozumleme kuralini tasir. Ornek: InitiatorManager veya SourceWarehouseManager.
    /// </summary>
    public string? ResolverKey { get; private set; }

    /// <summary>
    /// Workflow entity'leri mevcut mimaride navigation kullanan istisnai kume oldugu icin
    /// EF konfigurasyonu tanim iliskisini bu property uzerinden kurar.
    /// </summary>
    public virtual WorkflowDefinition WorkflowDefinition { get; private set; }

    private WorkflowStepDefinition()
    {
        WorkflowDefinition = default!;
    }

    public WorkflowStepDefinition(Guid id, Guid workflowDefinitionId, int stepOrder, string? requiredRoleName, string? resolverKey = null)
        : base(id)
    {
        WorkflowDefinitionId = workflowDefinitionId;
        StepOrder = stepOrder;
        RequiredRoleName = requiredRoleName;
        ResolverKey = resolverKey;
        WorkflowDefinition = default!;
    }
}
