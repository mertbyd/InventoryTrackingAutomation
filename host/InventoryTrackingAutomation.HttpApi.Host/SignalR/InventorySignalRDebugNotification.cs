using System;

namespace InventoryTrackingAutomation.SignalR;

/// <summary>
/// SignalR debug ekrani icin memory'de tutulan bildirim kaydi.
/// </summary>
public class InventorySignalRDebugNotification
{
    public Guid Id { get; set; }
    public string EventName { get; set; } = string.Empty;
    public Guid? TargetUserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public Guid? WorkflowInstanceId { get; set; }
    public Guid? WorkflowInstanceStepId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public bool Sent { get; set; }
    public string? Error { get; set; }
}
