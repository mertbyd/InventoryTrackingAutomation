namespace InventoryTrackingAutomation.Notifications;

public static partial class InventoryNotificationConstants
{
    /// <summary>
    /// Standart bildirim mesajlarini tutar.
    /// </summary>
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
