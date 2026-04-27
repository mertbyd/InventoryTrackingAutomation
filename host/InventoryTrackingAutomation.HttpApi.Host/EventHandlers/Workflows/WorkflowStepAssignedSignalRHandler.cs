using System.Threading.Tasks;
using InventoryTrackingAutomation.Events.Workflows;
using InventoryTrackingAutomation.Hubs;
using InventoryTrackingAutomation.SignalR;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace InventoryTrackingAutomation.EventHandlers.Workflows;

/// <summary>
/// Workflow adim atamalarini ilgili onayciya SignalR bildirimi olarak iletir.
/// </summary>
public class WorkflowStepAssignedSignalRHandler :
    ILocalEventHandler<WorkflowStepAssignedEto>,
    ITransientDependency
{
    private readonly IHubContext<InventoryNotificationHub> _hubContext;
    private readonly InventorySignalRDebugNotificationStore _debugStore;

    public WorkflowStepAssignedSignalRHandler(
        IHubContext<InventoryNotificationHub> hubContext,
        InventorySignalRDebugNotificationStore debugStore)
    {
        _hubContext = hubContext;
        _debugStore = debugStore;
    }

    public async Task HandleEventAsync(WorkflowStepAssignedEto eventData)
    {
        var createdAtUtc = System.DateTime.UtcNow;
        const string eventName = "ReceiveInventoryNotification";
        const string type = "WorkflowStepAssigned";
        const string title = "Yeni onay bekleniyor";
        const string message = "Bir hareket talebi onayınız için bekliyor.";

        if (!eventData.AssignedUserId.HasValue)
        {
            AddDebugNotification(
                eventData,
                eventName,
                type,
                title,
                message,
                createdAtUtc,
                sent: false,
                "AssignedUserId bos; workflow onaycisi cozulemedi.");
            return;
        }

        try
        {
            await _hubContext.Clients
                .User(eventData.AssignedUserId.Value.ToString())
                .SendAsync(eventName, new
                {
                    Type = type,
                    Title = title,
                    Message = message,
                    EntityType = eventData.EntityType,
                    EntityId = eventData.EntityId,
                    WorkflowInstanceId = eventData.WorkflowInstanceId,
                    WorkflowInstanceStepId = eventData.WorkflowInstanceStepId,
                    CreatedAt = createdAtUtc
                });

            AddDebugNotification(eventData, eventName, type, title, message, createdAtUtc, sent: true);
        }
        catch (System.Exception ex)
        {
            AddDebugNotification(eventData, eventName, type, title, message, createdAtUtc, sent: false, ex.Message);
            throw;
        }
    }

    private void AddDebugNotification(
        WorkflowStepAssignedEto eventData,
        string eventName,
        string type,
        string title,
        string message,
        System.DateTime createdAtUtc,
        bool sent,
        string? error = null)
    {
        _debugStore.Add(new InventorySignalRDebugNotification
        {
            Id = System.Guid.NewGuid(),
            EventName = eventName,
            TargetUserId = eventData.AssignedUserId,
            Type = type,
            Title = title,
            Message = message,
            EntityType = eventData.EntityType,
            EntityId = eventData.EntityId,
            WorkflowInstanceId = eventData.WorkflowInstanceId,
            WorkflowInstanceStepId = eventData.WorkflowInstanceStepId,
            CreatedAtUtc = createdAtUtc,
            Sent = sent,
            Error = error
        });
    }
}
