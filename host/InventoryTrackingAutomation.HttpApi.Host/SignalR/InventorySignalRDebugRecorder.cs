using System;
using InventoryTrackingAutomation.Notifications;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.SignalR;

/// <summary>
/// SignalR debug kayitlarini memory store'a yazan servis.
/// </summary>
public class InventorySignalRDebugRecorder : IInventorySignalRDebugRecorder, ITransientDependency
{
    private readonly InventorySignalRDebugNotificationStore _store;

    public InventorySignalRDebugRecorder(InventorySignalRDebugNotificationStore store)
    {
        _store = store;
    }

    public void Record(Guid? targetUserId, InventoryNotificationPayload payload, bool sent, string? error = null)
    {
        // Debug endpoint'in gosterecegi okunabilir SignalR deneme kaydini olusturur.
        _store.Add(new InventorySignalRDebugNotification
        {
            Id = Guid.NewGuid(),
            EventName = InventoryNotificationConstants.SignalREvents.ReceiveInventoryNotification,
            TargetUserId = targetUserId,
            Type = payload.Type,
            Title = payload.Title,
            Message = payload.Message,
            EntityType = payload.EntityType,
            EntityId = payload.EntityId,
            WorkflowInstanceId = payload.WorkflowInstanceId,
            WorkflowInstanceStepId = payload.WorkflowInstanceStepId,
            CreatedAtUtc = payload.CreatedAt,
            Sent = sent,
            Error = error
        });
    }
}
