using System;
using InventoryTrackingAutomation.Enums.Workflows;

namespace InventoryTrackingAutomation.Dtos.Workflows;

/// <summary>
/// İş akışı sürecinin anlık durumunu dönen DTO.
/// </summary>
public class WorkflowInstanceDto
{
    public Guid Id { get; set; }
    public Guid WorkflowDefinitionId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public WorkflowState State { get; set; }
    public Guid InitiatorUserId { get; set; }
}
