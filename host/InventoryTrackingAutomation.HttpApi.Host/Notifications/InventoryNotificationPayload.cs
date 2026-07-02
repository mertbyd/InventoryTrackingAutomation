using System;

namespace InventoryTrackingAutomation.Notifications;

/// <summary>
/// Client'a tasinan envanter bildirim mesaji; SignalR ve SSE tasiyicilarinin ortak payload'u.
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
    public DateTime CreatedAt { get; set; }
}
