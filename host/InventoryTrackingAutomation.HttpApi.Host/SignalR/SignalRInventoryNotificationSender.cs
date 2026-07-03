using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Notifications;
using InventoryTrackingAutomation.Hubs;
using InventoryTrackingAutomation.Interface.Notifications;
using InventoryTrackingAutomation.Notifications;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.SignalR;

/// <summary>
/// Envanter bildirimlerini SignalR hub uzerinden gonderen servis.
/// </summary>
public class SignalRInventoryNotificationSender : IInventoryNotificationSender, ITransientDependency
{
    private readonly IHubContext<InventoryNotificationHub> _hubContext;
    private readonly IInventorySignalRDebugRecorder _debugRecorder;

    public SignalRInventoryNotificationSender(
        IHubContext<InventoryNotificationHub> hubContext,
        IInventorySignalRDebugRecorder debugRecorder)
    {
        _hubContext = hubContext;
        _debugRecorder = debugRecorder;
    }

    public async Task SendToUserAsync(Guid userId, InventoryNotificationPayload payload)
    {
        // ABP SignalR user id olarak CurrentUser.Id degerini kullanir.
        await _hubContext.Clients
            .User(userId.ToString())
            .SendAsync(InventoryNotificationConstants.SignalREvents.ReceiveInventoryNotification, payload);

        // Development'ta son SignalR denemeleri debug store uzerinden izlenir.
        _debugRecorder.Record(userId, payload, sent: true);
    }
}
