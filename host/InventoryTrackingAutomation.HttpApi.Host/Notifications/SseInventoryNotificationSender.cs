using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Notifications;

/// <summary>
/// Envanter bildirimlerini acik SSE baglantilari uzerinden gonderen tasiyici.
/// </summary>
public class SseInventoryNotificationSender : IInventoryNotificationSender, ITransientDependency
{
    private readonly InventorySseConnectionManager _connectionManager;

    public SseInventoryNotificationSender(InventorySseConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public Task SendToUserAsync(Guid userId, InventoryNotificationPayload payload)
    {
        // Kanal yazimi senkron TryWrite ile biter; SSE stream'i payload'u kendi dongusunde client'a akitir.
        _connectionManager.SendToUser(userId, payload);
        return Task.CompletedTask;
    }
}
