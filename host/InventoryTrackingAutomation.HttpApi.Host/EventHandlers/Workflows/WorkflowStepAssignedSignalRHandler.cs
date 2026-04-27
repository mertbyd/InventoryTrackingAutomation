using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Events.Workflows;
using InventoryTrackingAutomation.Notifications;
using InventoryTrackingAutomation.SignalR;
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
    private readonly IInventoryNotificationSender _notificationSender;
    private readonly IInventorySignalRDebugRecorder _debugRecorder;

    public WorkflowStepAssignedSignalRHandler(
        IInventoryNotificationSender notificationSender,
        IInventorySignalRDebugRecorder debugRecorder)
    {
        _notificationSender = notificationSender;
        _debugRecorder = debugRecorder;
    }

    public async Task HandleEventAsync(WorkflowStepAssignedEto eventData)
    {
        // Workflow event'ini client tarafinin anlayacagi SignalR payload'una cevirir.
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
            // ABP SignalR UserIdProvider hedef kullaniciyi AssignedUserId ile eslestirir.
            await _notificationSender.SendToUserAsync(eventData.AssignedUserId.Value, payload);

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
