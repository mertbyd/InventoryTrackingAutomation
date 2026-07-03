using System;

namespace InventoryTrackingAutomation.Dtos.Notifications;

/// <summary>
/// Tum envanter bildirimlerinin ortak govdesi; tasiyicilarin (SSE, SignalR...) ortak payload base'i.
/// </summary>
// islevi: Bildirimin tipini, gosterim metnini ve iliskili entity baglamini tasir.
// sistemdeki gorevi: Ise ozel bildirimler bu base'i kalitip yalniz kendi alanlarini ekler; ortak cekirdek tek noktada tanimlidir.
public class InventoryNotificationPayload
{
    public string Type { get; set; } = string.Empty; // Bildirim tipi; InventoryNotificationConstants.Types sabitlerinden gelir.
    public string Title { get; set; } = string.Empty; // Client'ta gosterilecek baslik.
    public string Message { get; set; } = string.Empty; // Client'ta gosterilecek mesaj govdesi.
    public string EntityType { get; set; } = string.Empty; // Bildirimin isaret ettigi entity turu (navigasyon icin).
    public Guid EntityId { get; set; } // Bildirimin isaret ettigi entity kimligi.
    public DateTime CreatedAt { get; set; } // Bildirimin uretildigi an; generic handler base'i damgalar.
}
