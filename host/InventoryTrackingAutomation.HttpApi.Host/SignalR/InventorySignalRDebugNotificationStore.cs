using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace InventoryTrackingAutomation.SignalR;

/// <summary>
/// Development ortaminda SignalR bildirim denemelerini gorunur yapan memory store.
/// </summary>
public class InventorySignalRDebugNotificationStore
{
    private const int MaxItems = 100;
    private readonly ConcurrentQueue<InventorySignalRDebugNotification> _items = new();

    public void Add(InventorySignalRDebugNotification notification)
    {
        _items.Enqueue(notification);

        while (_items.Count > MaxItems && _items.TryDequeue(out _))
        {
        }
    }

    public IReadOnlyList<InventorySignalRDebugNotification> GetLatest()
    {
        return _items
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToList();
    }

    public void Clear()
    {
        while (_items.TryDequeue(out _))
        {
        }
    }
}
