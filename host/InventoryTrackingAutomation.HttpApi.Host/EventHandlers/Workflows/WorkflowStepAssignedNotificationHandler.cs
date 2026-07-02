using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Events.Workflows;
using InventoryTrackingAutomation.Notifications;
using InventoryTrackingAutomation.SignalR;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace InventoryTrackingAutomation.EventHandlers.Workflows;

/// <summary>
/// Workflow adim atamalarini kayitli tum bildirim tasiyicilarina (SignalR, SSE...) iletir.
/// </summary>
public class WorkflowStepAssignedNotificationHandler :
    ILocalEventHandler<WorkflowStepAssignedEto>,
    ITransientDependency
{
    private readonly IEnumerable<IInventoryNotificationSender> _notificationSenders;
    private readonly IInventorySignalRDebugRecorder _debugRecorder;

    public WorkflowStepAssignedNotificationHandler(
        IEnumerable<IInventoryNotificationSender> notificationSenders,
        IInventorySignalRDebugRecorder debugRecorder)
    {
        _notificationSenders = notificationSenders;
        _debugRecorder = debugRecorder;
    }

    public async Task HandleEventAsync(WorkflowStepAssignedEto eventData)
    {
        // Workflow event'ini client tarafinin anlayacagi bildirim payload'una cevirir.
        var payload = CreatePayload(eventData);

        if (!eventData.AssignedUserId.HasValue)
        {
            _debugRecorder.Record(
                eventData.AssignedUserId,
                payload,
                sent: false,
                InventoryNotificationConstants.Messages.MissingAssignedUser);
            return;
        }

        try
        {
            // Bildirim kanal bagimsizdir; kayitli her tasiyici (SignalR, SSE...) ayni payload'u kendi kanalindan gonderir.
            foreach (var notificationSender in _notificationSenders)
            {
                await notificationSender.SendToUserAsync(eventData.AssignedUserId.Value, payload);
            }

            _debugRecorder.Record(eventData.AssignedUserId, payload, sent: true);
        }
        catch (Exception ex)
        {
            _debugRecorder.Record(eventData.AssignedUserId, payload, sent: false, ex.Message);
            throw;
        }
    }

    private static InventoryNotificationPayload CreatePayload(WorkflowStepAssignedEto eventData)
    {
        // Bildirim metinleri ve event tipi merkezi sabitlerden gelir.
        return new InventoryNotificationPayload
        {
            Type = InventoryNotificationConstants.Types.WorkflowStepAssigned,
            Title = InventoryNotificationConstants.Messages.WorkflowStepAssignedTitle,
            Message = InventoryNotificationConstants.Messages.WorkflowStepAssignedMessage,
            EntityType = eventData.EntityType,
            EntityId = eventData.EntityId,
            WorkflowInstanceId = eventData.WorkflowInstanceId,
            WorkflowInstanceStepId = eventData.WorkflowInstanceStepId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
