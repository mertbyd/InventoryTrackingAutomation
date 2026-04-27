namespace InventoryTrackingAutomation.Notifications;

/// <summary>
/// Canli bildirimlerde kullanilan sabit event ve tip adlari.
/// </summary>
public static class InventoryNotificationConstants
{
    public static class SignalREvents
    {
        // Client tarafinda dinlenecek SignalR event adi.
        public const string ReceiveInventoryNotification = "ReceiveInventoryNotification";
    }

    public static class Types
    {
        // Workflow adimi bir onayciya atandiginda gonderilen bildirim tipi.
        public const string WorkflowStepAssigned = "WorkflowStepAssigned";
    }

    public static class Messages
    {
        // Onayciya gosterilecek standart workflow atama basligi.
        public const string WorkflowStepAssignedTitle = "Yeni onay bekleniyor";

        // Onayciya gosterilecek standart workflow atama mesaji.
        public const string WorkflowStepAssignedMessage = "Bir hareket talebi onayiniz icin bekliyor.";

        // Workflow onaycisi cozulemezse debug kaydinda gorunen hata.
        public const string MissingAssignedUser = "AssignedUserId bos; workflow onaycisi cozulemedi.";
    }
}
