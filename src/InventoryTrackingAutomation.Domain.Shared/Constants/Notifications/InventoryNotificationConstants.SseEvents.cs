namespace InventoryTrackingAutomation.Notifications;

public static partial class InventoryNotificationConstants
{
    /// <summary>
    /// SSE (Server-Sent Events) stream endpoint'ini ve event adlarini tutar.
    /// </summary>
    public static class SseEvents
    {
        // Client'in EventSource/fetch ile baglanacagi tek yonlu bildirim stream endpoint'i.
        public const string StreamPath = "/api/notifications/stream";

        // Client tarafinda dinlenecek SSE event adi (SignalREvents.ReceiveInventoryNotification'in SSE karsiligi).
        public const string InventoryNotification = "inventory-notification";

        // Sessiz donemde baglantiyi canli tutmak icin atilan bos event; client bu adi gorunce yok sayar.
        public const string Heartbeat = "heartbeat";
    }
}
