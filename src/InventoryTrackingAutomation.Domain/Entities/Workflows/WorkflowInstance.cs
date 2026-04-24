using System;
using System.Collections.Generic;
using InventoryTrackingAutomation.Enums.Workflows;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Workflows;

/// <summary>
/// Başlatılmış bir iş akışı sürecini (örneğini) temsil eden entity.
/// </summary>
public class WorkflowInstance : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// Hangi iş akışı tanımından üretildiği.
    /// </summary>
    public Guid WorkflowDefinitionId { get; private set; }

    /// <summary>
    /// Bu iş akışının hangi entity türü için başlatıldığı (Örn: "MovementRequest").
    /// </summary>
    public string EntityType { get; private set; }

    /// <summary>
    /// İş akışına tabi olan entity'nin Id'si.
    /// </summary>
    public Guid EntityId { get; private set; }

    /// <summary>
    /// İş akışının anlık durumu.
    /// </summary>
    public WorkflowState State { get; internal set; }

    /// <summary>
    /// İş akışını başlatan kullanıcının Id'si.
    /// </summary>
    public Guid InitiatorUserId { get; private set; }

    /// <summary>
    /// İş akışı tanımı navigation property'si.
    /// </summary>
    public virtual WorkflowDefinition WorkflowDefinition { get; private set; }

    /// <summary>
    /// İş akışına ait adımlar.
    /// </summary>
    public virtual ICollection<WorkflowInstanceStep> Steps { get; private set; }

    private WorkflowInstance()
    {
    }

    public WorkflowInstance(Guid id, Guid workflowDefinitionId, string entityType, Guid entityId, WorkflowState state, Guid initiatorUserId)
        : base(id)
    {
        WorkflowDefinitionId = workflowDefinitionId;
        EntityType = entityType;
        EntityId = entityId;
        State = state;
        InitiatorUserId = initiatorUserId;
        Steps = new List<WorkflowInstanceStep>();
    }
}
