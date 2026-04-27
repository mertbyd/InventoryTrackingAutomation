using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Hubs;
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

    public SignalRInventoryNotificationSender(IHubContext<InventoryNotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task SendToUserAsync(Guid userId, InventoryNotificationPayload payload)
    {
        // ABP SignalR user id olarak CurrentUser.Id degerini kullanir.
        return _hubContext.Clients
            .User(userId.ToString())
            .SendAsync(InventoryNotificationConstants.SignalREvents.ReceiveInventoryNotification, payload);
    }
}
