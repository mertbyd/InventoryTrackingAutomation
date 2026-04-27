using System;
using InventoryTrackingAutomation.Enums.Workflows;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Workflows;

/// <summary>
/// Başlatılmış bir iş akışı sürecinin onay adımlarını temsil eden entity.
/// </summary>
public class WorkflowInstanceStep : AuditedEntity<Guid>
{
    /// <summary>
    /// Bağlı olduğu iş akışı süreci Id'si.
    /// </summary>
    public Guid WorkflowInstanceId { get; private set; }

    /// <summary>
    /// Hangi tanımdan üretildiği.
    /// </summary>
    public Guid WorkflowStepDefinitionId { get; private set; }

    /// <summary>
    /// Bu adımı onaylamak üzere atanmış spesifik kullanıcı Id'si (ResolverKey üzerinden çözüldüğünde dolu olur).
    /// </summary>
    public Guid? AssignedUserId { get; private set; }

    /// <summary>
    /// Adımın durumu / Alınan aksiyon.
    /// </summary>
    public WorkflowActionType ActionTaken { get; internal set; }

    /// <summary>
    /// Onaylarken/Reddederken girilen not.
    /// </summary>
    public string? Note { get; internal set; }

    /// <summary>
    /// Aksiyonun alındığı tarih.
    /// </summary>
    public DateTime? ActionDate { get; internal set; }

    /// <summary>
    /// Bağlı olduğu iş akışı süreci navigation property'si.
    /// </summary>
    public virtual WorkflowInstance WorkflowInstance { get; private set; }

    /// <summary>
    /// İş akışı adım tanımı navigation property'si.
    /// </summary>
    public virtual WorkflowStepDefinition WorkflowStepDefinition { get; private set; }

    private WorkflowInstanceStep()
    {
    }

    public WorkflowInstanceStep(Guid id, Guid workflowInstanceId, Guid workflowStepDefinitionId, Guid? assignedUserId, WorkflowActionType actionTaken = WorkflowActionType.Pending)
        : base(id)
    {
        WorkflowInstanceId = workflowInstanceId;
        WorkflowStepDefinitionId = workflowStepDefinitionId;
        AssignedUserId = assignedUserId;
        ActionTaken = actionTaken;
    }
}
