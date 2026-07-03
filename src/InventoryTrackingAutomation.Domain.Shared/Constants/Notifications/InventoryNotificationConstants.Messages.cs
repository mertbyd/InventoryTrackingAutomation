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

        // Depo sorumlusuna gosterilecek standart urun cikisi basligi.
        public const string WarehouseStockDispatchedTitle = "Depodan urun cikisi";

        // Depo sorumlusuna gosterilecek standart urun cikisi mesaji.
        public const string WarehouseStockDispatchedMessage = "Sorumlusu oldugunuz depodan urun cikisi gerceklesti.";
    }
}
