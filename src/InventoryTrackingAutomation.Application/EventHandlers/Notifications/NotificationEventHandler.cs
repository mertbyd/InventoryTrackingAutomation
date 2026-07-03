using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Notifications;
using InventoryTrackingAutomation.Interface.Notifications;
using Volo.Abp.EventBus;

namespace InventoryTrackingAutomation.Application.EventHandlers.Notifications;

/// <summary>
/// Bildirim ureten event handler'larin generic base'i; gonderim akisini tek yerde toplar.
/// </summary>
// islevi: Hedef kullaniciyi cozer, payload'u uretip zaman damgalar ve kayitli tum tasiyicilara (SSE, SignalR...) gonderir.
// sistemdeki gorevi: Yeni bir bildirim eklemek = bu base'i kalitip iki hook doldurmak; gonderim/fan-out mantigi tekrar yazilmaz.
public abstract class NotificationEventHandler<TEvent> : ILocalEventHandler<TEvent>
    where TEvent : class
{
    private readonly IEnumerable<IInventoryNotificationSender> _notificationSenders;

    protected NotificationEventHandler(IEnumerable<IInventoryNotificationSender> notificationSenders)
    {
        _notificationSenders = notificationSenders;
    }

    public async Task HandleEventAsync(TEvent eventData)
    {
        // Hedef kullanici cozulemiyorsa (event bildirimlik degilse veya sorumlu yoksa) sessizce gecilir.
        var targetUserId = await ResolveTargetUserIdAsync(eventData);
        if (!targetUserId.HasValue)
        {
            return;
        }

        var payload = CreatePayload(eventData);
        payload.CreatedAt = DateTime.UtcNow;

        // Bildirim kanal bagimsizdir; kayitli her tasiyici ayni payload'u kendi kanalindan gonderir.
        foreach (var notificationSender in _notificationSenders)
        {
            await notificationSender.SendToUserAsync(targetUserId.Value, payload);
        }
    }

    /// <summary>
    /// Event'ten bildirimin gidecegi ABP user id'sini cozer; null donerse bildirim uretilmez.
    /// </summary>
    protected abstract Task<Guid?> ResolveTargetUserIdAsync(TEvent eventData);

    /// <summary>
    /// Event'i client'in anlayacagi bildirim payload'una cevirir; CreatedAt base tarafindan damgalanir.
    /// </summary>
    protected abstract InventoryNotificationPayload CreatePayload(TEvent eventData);
}
