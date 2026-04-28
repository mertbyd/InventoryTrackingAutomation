namespace InventoryTrackingAutomation.Settings;

/// <summary>
/// Projedeki tum setting anahtarlarini merkezi olarak tutan sabitler sinifi.
/// </summary>
public static class InventoryTrackingAutomationSettings
{
    private const string Prefix = "InventoryTrackingAutomation";

    /// <summary>
    /// Is akisi modulune ait setting anahtarlari.
    /// </summary>
    public static class Workflows
    {
        public const string AllowedStates = Prefix + ".Workflows.AllowedStates";
        public const string AllowedActions = Prefix + ".Workflows.AllowedActions";
    }

    /// <summary>
    /// Urun modulune ait UnitTypeEnum setting anahtarlari.
    /// </summary>
    public static class Products
    {
        public const string DefaultUnitType = Prefix + ".Products.DefaultUnitType";   // Varsayilan olcu birimi. Ornek: "1" (Piece)
        public const string AllowedUnitTypes = Prefix + ".Products.AllowedUnitTypes"; // Izin verilen olcu birimleri. Ornek: "1,2,3,4,5"
    }

    /// <summary>
    /// Lokasyon modulune ait SiteTypeEnum setting anahtarlari.
    /// </summary>
    public static class Sites
    {
        public const string DefaultSiteType = Prefix + ".Sites.DefaultSiteType";   // Varsayilan lokasyon tipi. Ornek: "1" (Warehouse)
        public const string AllowedSiteTypes = Prefix + ".Sites.AllowedSiteTypes"; // Izin verilen lokasyon tipleri. Ornek: "1,2,3,4"
    }

    /// <summary>
    /// Arac modulune ait VehicleTypeEnum setting anahtarlari.
    /// </summary>
    public static class Vehicles
    {
        public const string DefaultVehicleType = Prefix + ".Vehicles.DefaultVehicleType";   // Varsayilan arac tipi. Ornek: "1" (Truck)
        public const string AllowedVehicleTypes = Prefix + ".Vehicles.AllowedVehicleTypes"; // Izin verilen arac tipleri. Ornek: "1,2,3"
    }

    /// <summary>
    /// Calisan modulune ait WorkerTypeEnum setting anahtarlari.
    /// </summary>
    public static class Workers
    {
        public const string DefaultWorkerType = Prefix + ".Workers.DefaultWorkerType";   // Varsayilan calisan tipi. Ornek: "2" (BlueCollar)
        public const string AllowedWorkerTypes = Prefix + ".Workers.AllowedWorkerTypes"; // Izin verilen calisan tipleri. Ornek: "1,2,3"
    }

    /// <summary>
    /// Hareket talebi modulune ait status ve priority setting anahtarlari.
    /// </summary>
    public static class MovementRequests
    {
        public const string DefaultStatus = Prefix + ".MovementRequests.DefaultStatus";       // Varsayilan talep durumu. Ornek: "1" (Pending)
        public const string AllowedStatuses = Prefix + ".MovementRequests.AllowedStatuses";   // Izin verilen talep durumlari. Ornek: "1,2,3,4,5,6"
        public const string DefaultPriority = Prefix + ".MovementRequests.DefaultPriority";    // Varsayilan talep onceligi. Ornek: "2" (Normal)
        public const string AllowedPriorities = Prefix + ".MovementRequests.AllowedPriorities"; // Izin verilen talep oncelikleri. Ornek: "1,2,3,4"
    }

    /// <summary>
    /// Stok hareketi modulune ait StockMovementTypeEnum setting anahtarlari.
    /// </summary>
    public static class StockMovements
    {
        public const string DefaultType = Prefix + ".StockMovements.DefaultType";  // Varsayilan stok hareket tipi. Ornek: "1" (In)
        public const string AllowedTypes = Prefix + ".StockMovements.AllowedTypes"; // Izin verilen stok hareket tipleri.
    }

    /// <summary>
    /// Manager ve SettingDefinitionProvider'larin kullandigi geriye donuk uyumlu erisim noktalari.
    /// </summary>
    public static class Masters
    {
        public const string AllowedUnitTypes = Products.AllowedUnitTypes;
        public const string AllowedWorkerTypes = Workers.AllowedWorkerTypes;
        public const string AllowedSiteTypes = Sites.AllowedSiteTypes;
        public const string AllowedVehicleTypes = Vehicles.AllowedVehicleTypes;
    }

    /// <summary>
    /// Domain manager'larin kullandigi Movements erisim noktalari.
    /// </summary>
    public static class Movements
    {
        public const string AllowedMovementPriorities = MovementRequests.AllowedPriorities;
        public const string AllowedMovementStatuses = MovementRequests.AllowedStatuses;
        public const string AllowedApprovalStatuses = Prefix + ".Movements.AllowedApprovalStatuses";
    }

    /// <summary>
    /// Domain manager'larin kullandigi Stock erisim noktalari.
    /// </summary>
    public static class Stock
    {
        public const string AllowedStockMovementTypes = StockMovements.AllowedTypes;
    }
}
