using System;
using System.Threading.Tasks;

namespace InventoryTrackingAutomation.Notifications;

/// <summary>
/// Envanter bildirimlerini hedef kullaniciya ileten tasiyici soyutlamasi.
/// </summary>
// islevi: Bildirimin hangi kanaldan (SignalR, SSE...) gidecegini implementasyona birakir.
// sistemdeki gorevi: Event handler'lar tum kayitli tasiyicilara gonderir; yeni kanal eklemek yeni bir implementasyon eklemekten ibarettir.
public interface IInventoryNotificationSender
{
    // Bildirimi sadece verilen ABP user id'sine bagli client'lara yollar.
    Task SendToUserAsync(Guid userId, InventoryNotificationPayload payload);
}
