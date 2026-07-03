using System;
using System.Threading.Channels;
using InventoryTrackingAutomation.Dtos.Notifications;

namespace InventoryTrackingAutomation.Notifications;

/// <summary>
/// Tek bir SSE baglantisinin okuma kanali; dispose edildiginde baglanti kayittan dusulur.
/// </summary>
public sealed class InventorySseSubscription : IDisposable
{
    private readonly InventorySseConnectionManager _connectionManager;
    private readonly Guid _userId;
    private readonly Guid _connectionId;

    public ChannelReader<InventoryNotificationPayload> Reader { get; }

    internal InventorySseSubscription(
        InventorySseConnectionManager connectionManager,
        Guid userId,
        Guid connectionId,
        ChannelReader<InventoryNotificationPayload> reader)
    {
        _connectionManager = connectionManager;
        _userId = userId;
        _connectionId = connectionId;
        Reader = reader;
    }

    public void Dispose()
    {
        _connectionManager.Unsubscribe(_userId, _connectionId);
    }
}
