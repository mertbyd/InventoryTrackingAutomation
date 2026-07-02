using System;
using System.Collections.Concurrent;
using System.Threading.Channels;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Notifications;

/// <summary>
/// Acik SSE baglantilarinin kullanici bazli kayit defteri.
/// </summary>
// islevi: Kullanici basina acik SSE kanallarini tutar; bildirim geldiginde kullanicinin tum kanallarina yazar.
// sistemdeki gorevi: SseInventoryNotificationSender ile stream endpoint'i arasindaki tek kopru; baglanti yasam dongusu subscription dispose ile kapanir.
public class InventorySseConnectionManager : ISingletonDependency
{
    // Ayni kullanici birden fazla sekme/cihazdan baglanabilir; her baglanti kendi kanalini alir.
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, Channel<InventoryNotificationPayload>>> _connections = new();

    /// <summary>
    /// Kullanici icin yeni bir SSE kanali acar; baglanti kapaninca subscription dispose edilerek kayit silinir.
    /// </summary>
    public InventorySseSubscription Subscribe(Guid userId)
    {
        var connectionId = Guid.NewGuid();
        var channel = Channel.CreateUnbounded<InventoryNotificationPayload>();

        var userChannels = _connections.GetOrAdd(
            userId,
            _ => new ConcurrentDictionary<Guid, Channel<InventoryNotificationPayload>>());
        userChannels[connectionId] = channel;

        return new InventorySseSubscription(this, userId, connectionId, channel.Reader);
    }

    /// <summary>
    /// Bildirimi kullanicinin acik tum SSE kanallarina yazar; baglantisi yoksa sessizce gecilir.
    /// </summary>
    public void SendToUser(Guid userId, InventoryNotificationPayload payload)
    {
        if (!_connections.TryGetValue(userId, out var userChannels))
        {
            return;
        }

        foreach (var channel in userChannels.Values)
        {
            channel.Writer.TryWrite(payload);
        }
    }

    internal void Unsubscribe(Guid userId, Guid connectionId)
    {
        if (!_connections.TryGetValue(userId, out var userChannels))
        {
            return;
        }

        userChannels.TryRemove(connectionId, out _);
        if (userChannels.IsEmpty)
        {
            _connections.TryRemove(userId, out _);
        }
    }
}
