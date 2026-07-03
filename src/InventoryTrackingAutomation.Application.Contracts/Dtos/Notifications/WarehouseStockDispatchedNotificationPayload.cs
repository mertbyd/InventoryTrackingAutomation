using System;

namespace InventoryTrackingAutomation.Dtos.Notifications;

/// <summary>
/// Depodan urun cikisi bildiriminin payload'u; ortak cekirdege kaynak depo baglamini ekler.
/// </summary>
// islevi: Depo sorumlusunun hangi deposundan cikis oldugunu gormesi icin kaynak depo kimligini tasir.
// sistemdeki gorevi: Sevkiyat bildirimine ozel alanlar yalniz bu tipte durur; ortak alanlar base'ten kalitilir.
public class WarehouseStockDispatchedNotificationPayload : InventoryNotificationPayload
{
    public Guid SourceWarehouseId { get; set; } // Cikisin gerceklestigi kaynak depo kimligi.
}
