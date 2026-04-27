using System;

namespace InventoryTrackingAutomation.SignalR;

/// <summary>
/// SignalR ile client'a tasinan envanter bildirim mesaji.
/// </summary>
public class InventoryNotificationPayload
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public Guid WorkflowInstanceId { get; set; }
    public Guid WorkflowInstanceStepId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
