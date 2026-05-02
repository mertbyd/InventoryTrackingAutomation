using System;
using System.Collections.Generic;
using InventoryTrackingAutomation.Enums.Workflows;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Workflows;

/// <summary>
/// Baslatilmis bir is akisi surecini temsil eden aggregate root.
/// </summary>
public class WorkflowInstance : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// Surecin hangi is akisi tanimindan uretildigini tasir.
    /// </summary>
    public Guid WorkflowDefinitionId { get; private set; }

    /// <summary>
    /// Is akisinin hangi entity tipi icin calistigini tasir. Ornek: MovementRequest.
    /// </summary>
    public string EntityType { get; private set; }

    /// <summary>
    /// Is akisina tabi olan entity kimligini tasir.
    /// </summary>
    public Guid EntityId { get; private set; }

    /// <summary>
    /// Surecin anlik durumunu belirler.
    /// </summary>
    public WorkflowState State { get; internal set; }

    /// <summary>
    /// Is akisini baslatan kullanicinin kimligini tasir.
    /// </summary>
    public Guid InitiatorUserId { get; private set; }

    /// <summary>
    /// Workflow entity'leri mevcut mimaride navigation kullanan istisnai kume oldugu icin
    /// EF konfigurasyonu tanim iliskisini bu property uzerinden kurar.
    /// </summary>
    public virtual WorkflowDefinition WorkflowDefinition { get; private set; }

    /// <summary>
    /// Surece ait islenebilir adimlari tasir.
    /// </summary>
    public virtual ICollection<WorkflowInstanceStep> Steps { get; private set; }

    private WorkflowInstance()
    {
        EntityType = string.Empty;
        WorkflowDefinition = default!;
        Steps = new List<WorkflowInstanceStep>();
    }

    public WorkflowInstance(Guid id, Guid workflowDefinitionId, string entityType, Guid entityId, WorkflowState state, Guid initiatorUserId)
        : base(id)
    {
        WorkflowDefinitionId = workflowDefinitionId;
        EntityType = entityType;
        EntityId = entityId;
        State = state;
        InitiatorUserId = initiatorUserId;
        WorkflowDefinition = default!;
        Steps = new List<WorkflowInstanceStep>();
    }
}
