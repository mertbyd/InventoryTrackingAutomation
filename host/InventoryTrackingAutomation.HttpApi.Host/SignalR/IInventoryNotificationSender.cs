using System;
using System.Threading.Tasks;

namespace InventoryTrackingAutomation.SignalR;

/// <summary>
/// Envanter bildirimlerini hedef kullaniciya ileten soyutlama.
/// </summary>
public interface IInventoryNotificationSender
{
    // Bildirimi sadece verilen ABP user id'sine bagli SignalR client'lara yollar.
    Task SendToUserAsync(Guid userId, InventoryNotificationPayload payload);
}
