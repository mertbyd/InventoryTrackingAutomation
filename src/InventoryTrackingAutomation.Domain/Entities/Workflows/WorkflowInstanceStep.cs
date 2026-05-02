using System;
using InventoryTrackingAutomation.Enums.Workflows;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Workflows;

/// <summary>
/// Baslatilmis bir is akisi surecinin tek bir onay adimini temsil eden entity.
/// </summary>
public class WorkflowInstanceStep : AuditedEntity<Guid>
{
    /// <summary>
    /// Bagli oldugu is akisi surecinin kimligini tasir.
    /// </summary>
    public Guid WorkflowInstanceId { get; private set; }

    /// <summary>
    /// Bu adimin hangi sablon adimindan uretildigini tasir.
    /// </summary>
    public Guid WorkflowStepDefinitionId { get; private set; }

    /// <summary>
    /// ResolverKey ile belirlenen spesifik onayci kullanici kimligini tasir.
    /// </summary>
    public Guid? AssignedUserId { get; private set; }

    /// <summary>
    /// Adimda bekleyen veya alinmis aksiyonu belirler.
    /// </summary>
    public WorkflowActionType ActionTaken { get; internal set; }

    /// <summary>
    /// Onay veya red sirasinda girilen notu tasir.
    /// </summary>
    public string? Note { get; internal set; }

    /// <summary>
    /// Aksiyonun alinma zamanini tasir.
    /// </summary>
    public DateTime? ActionDate { get; internal set; }

    /// <summary>
    /// Workflow entity'leri mevcut mimaride navigation kullanan istisnai kume oldugu icin
    /// EF konfigurasyonu surec iliskisini bu property uzerinden kurar.
    /// </summary>
    public virtual WorkflowInstance WorkflowInstance { get; private set; }

    /// <summary>
    /// Workflow entity'leri mevcut mimaride navigation kullanan istisnai kume oldugu icin
    /// EF konfigurasyonu sablon adim iliskisini bu property uzerinden kurar.
    /// </summary>
    public virtual WorkflowStepDefinition WorkflowStepDefinition { get; private set; }

    private WorkflowInstanceStep()
    {
        WorkflowInstance = default!;
        WorkflowStepDefinition = default!;
    }

    public WorkflowInstanceStep(Guid id, Guid workflowInstanceId, Guid workflowStepDefinitionId, Guid? assignedUserId, WorkflowActionType actionTaken = WorkflowActionType.Pending)
        : base(id)
    {
        WorkflowInstanceId = workflowInstanceId;
        WorkflowStepDefinitionId = workflowStepDefinitionId;
        AssignedUserId = assignedUserId;
        ActionTaken = actionTaken;
        WorkflowInstance = default!;
        WorkflowStepDefinition = default!;
    }
}
