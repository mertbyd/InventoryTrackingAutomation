using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Notifications;

namespace InventoryTrackingAutomation.Interface.Notifications;

/// <summary>
/// Envanter bildirimlerini hedef kullaniciya ileten tasiyici soyutlamasi.
/// </summary>
// islevi: Bildirimin hangi kanaldan (SSE, SignalR...) gidecegini implementasyona birakir.
// sistemdeki gorevi: Event handler'lar kayitli tum tasiyicilara gonderir; yeni kanal eklemek yeni bir implementasyon eklemekten ibarettir.
public interface IInventoryNotificationSender
{
    /// <summary>
    /// Bildirimi yalnizca verilen ABP user id'sine bagli client'lara yollar.
    /// </summary>
    Task SendToUserAsync(Guid userId, InventoryNotificationPayload payload);
}
