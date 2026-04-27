using System;

namespace InventoryTrackingAutomation.SignalR;

/// <summary>
/// SignalR bildirim denemelerini debug store'a kaydeden soyutlama.
/// </summary>
public interface IInventorySignalRDebugRecorder
{
    // Bildirimin kime, hangi payload ile ve hangi sonuc ile denendigini kaydeder.
    void Record(Guid? targetUserId, InventoryNotificationPayload payload, bool sent, string? error = null);
}
