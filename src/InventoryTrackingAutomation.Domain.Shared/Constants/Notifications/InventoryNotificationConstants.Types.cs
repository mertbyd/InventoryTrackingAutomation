namespace InventoryTrackingAutomation.Notifications;

public static partial class InventoryNotificationConstants
{
    /// <summary>
    /// Bildirim tip adlarini tutar.
    /// </summary>
    public static class Types
    {
        // Workflow adimi bir onayciya atandiginda gonderilen bildirim tipi.
        public const string WorkflowStepAssigned = "WorkflowStepAssigned";

        // Depodan urun cikisi gerceklestiginde depo sorumlusuna gonderilen bildirim tipi.
        public const string WarehouseStockDispatched = "WarehouseStockDispatched";
    }
}
