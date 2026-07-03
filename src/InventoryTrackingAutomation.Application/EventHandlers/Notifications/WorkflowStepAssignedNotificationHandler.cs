using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Application.Mappers.Notifications;
using InventoryTrackingAutomation.Dtos.Notifications;
using InventoryTrackingAutomation.Events.Workflows;
using InventoryTrackingAutomation.Interface.Notifications;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.EventHandlers.Notifications;

/// <summary>
/// Workflow adimi bir onayciya atandiginda onayciya bildirim gonderir.
/// </summary>
public class WorkflowStepAssignedNotificationHandler :
    NotificationEventHandler<WorkflowStepAssignedEto>,
    ITransientDependency
{
    private static readonly InventoryNotificationMapper _mapper = new();

    public WorkflowStepAssignedNotificationHandler(IEnumerable<IInventoryNotificationSender> notificationSenders)
        : base(notificationSenders)
    {
    }

    // Hedef, event'in uzerinde hazir gelir; atanmis onayci yoksa bildirim uretilmez.
    protected override Task<Guid?> ResolveTargetUserIdAsync(WorkflowStepAssignedEto eventData)
    {
        return Task.FromResult(eventData.AssignedUserId);
    }

    protected override InventoryNotificationPayload CreatePayload(WorkflowStepAssignedEto eventData)
    {
        return _mapper.MapToPayload(eventData);
    }
}
