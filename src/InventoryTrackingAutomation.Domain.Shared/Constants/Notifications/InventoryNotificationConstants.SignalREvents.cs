namespace InventoryTrackingAutomation.Notifications;

public static partial class InventoryNotificationConstants
{
    /// <summary>
    /// SignalR client event adlarini tutar.
    /// </summary>
    public static class SignalREvents
    {
        // Client tarafinda dinlenecek SignalR event adi.
        public const string ReceiveInventoryNotification = "ReceiveInventoryNotification";
    }
}
