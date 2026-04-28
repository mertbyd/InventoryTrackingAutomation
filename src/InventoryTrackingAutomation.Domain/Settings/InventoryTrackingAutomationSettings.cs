namespace InventoryTrackingAutomation.Settings;

public static class InventoryTrackingAutomationSettings
{
    private const string Prefix = "InventoryTrackingAutomation";

    // İş Akışı Ayarları
    public static class Workflows
    {
        public const string AllowedStates = Prefix + ".Workflows.AllowedStates";
        public const string AllowedActions = Prefix + ".Workflows.AllowedActions";
    }

    // Ürün Ayarları
    public static class Products
    {
        public const string DefaultUnitType = Prefix + ".Products.DefaultUnitType";
        public const string AllowedUnitTypes = Prefix + ".Products.AllowedUnitTypes";
    }

    // Araç Ayarları
    public static class Vehicles
    {
        public const string DefaultVehicleType = Prefix + ".Vehicles.DefaultVehicleType";
        public const string AllowedVehicleTypes = Prefix + ".Vehicles.AllowedVehicleTypes";
    }

    // Çalışan Ayarları
    public static class Workers
    {
        public const string DefaultWorkerType = Prefix + ".Workers.DefaultWorkerType";
        public const string AllowedWorkerTypes = Prefix + ".Workers.AllowedWorkerTypes";
    }

    // Hareket Talebi Ayarları
    public static class MovementRequests
    {
        public const string DefaultStatus = Prefix + ".MovementRequests.DefaultStatus";
        public const string AllowedStatuses = Prefix + ".MovementRequests.AllowedStatuses";
        public const string DefaultPriority = Prefix + ".MovementRequests.DefaultPriority";
        public const string AllowedPriorities = Prefix + ".MovementRequests.AllowedPriorities";
    }

    // Onay ve Hareket Ayarları
    public static class Movements
    {
        public const string AllowedApprovalStatuses = Prefix + ".Movements.AllowedApprovalStatuses";
        public const string AllowedMovementPriorities = MovementRequests.AllowedPriorities;
        public const string AllowedMovementStatuses = MovementRequests.AllowedStatuses;
    }

    // Envanter İşlem Ayarları
    public static class InventoryTransactions
    {
        public const string DefaultType = Prefix + ".InventoryTransactions.DefaultType";
        public const string AllowedTypes = Prefix + ".InventoryTransactions.AllowedTypes";
    }

    // Geriye Dönük Uyumluluk Alias'ları
    public static class Masters
    {
        public const string AllowedUnitTypes = Products.AllowedUnitTypes;
        public const string AllowedWorkerTypes = Workers.AllowedWorkerTypes;
        public const string AllowedVehicleTypes = Vehicles.AllowedVehicleTypes;
    }

    public static class Stock
    {
        public const string AllowedInventoryTransactionTypes = InventoryTransactions.AllowedTypes;
    }
}
