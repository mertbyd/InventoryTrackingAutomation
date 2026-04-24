namespace InventoryTrackingAutomation.Settings;

/// <summary>
/// Projedeki tüm setting anahtarlarını merkezi olarak tutan sabitler sınıfı.
/// </summary>
public static class InventoryTrackingAutomationSettings
{
    private const string Prefix = "InventoryTrackingAutomation";

    /// <summary>
    /// İş akışı modülüne ait setting anahtarları.
    /// </summary>
    public static class Workflows
    {
        public const string AllowedStates = Prefix + ".Workflows.AllowedStates";
        public const string AllowedActions = Prefix + ".Workflows.AllowedActions";
    }

    /// <summary>
    /// Ürün modülüne ait UnitTypeEnum setting anahtarları.
    /// </summary>
    public static class Products
    {
        public const string DefaultUnitType  = Prefix + ".Products.DefaultUnitType";   // Varsayılan ölçü birimi. Örnek değer: "1" (Piece)
        public const string AllowedUnitTypes = Prefix + ".Products.AllowedUnitTypes";  // İzin verilen ölçü birimleri. Örnek değer: "1,2,3,4,5"
    }

    /// <summary>
    /// Lokasyon modülüne ait SiteTypeEnum setting anahtarları.
    /// </summary>
    public static class Sites
    {
        public const string DefaultSiteType  = Prefix + ".Sites.DefaultSiteType";   // Varsayılan lokasyon tipi. Örnek değer: "1" (Warehouse)
        public const string AllowedSiteTypes = Prefix + ".Sites.AllowedSiteTypes";  // İzin verilen lokasyon tipleri. Örnek değer: "1,2,3,4"
    }

    /// <summary>
    /// Araç modülüne ait VehicleTypeEnum setting anahtarları.
    /// </summary>
    public static class Vehicles
    {
        public const string DefaultVehicleType  = Prefix + ".Vehicles.DefaultVehicleType";   // Varsayılan araç tipi. Örnek değer: "1" (Truck)
        public const string AllowedVehicleTypes = Prefix + ".Vehicles.AllowedVehicleTypes";  // İzin verilen araç tipleri. Örnek değer: "1,2,3"
    }

    /// <summary>
    /// Çalışan modülüne ait WorkerTypeEnum setting anahtarları.
    /// </summary>
    public static class Workers
    {
        public const string DefaultWorkerType  = Prefix + ".Workers.DefaultWorkerType";   // Varsayılan çalışan tipi. Örnek değer: "2" (BlueCollar)
        public const string AllowedWorkerTypes = Prefix + ".Workers.AllowedWorkerTypes";  // İzin verilen çalışan tipleri. Örnek değer: "1,2,3"
    }

    /// <summary>
    /// Hareket talebi modülüne ait MovementStatusEnum ve MovementPriorityEnum setting anahtarları.
    /// </summary>
    public static class MovementRequests
    {
        public const string DefaultStatus      = Prefix + ".MovementRequests.DefaultStatus";       // Varsayılan talep durumu. Örnek değer: "1" (Pending)
        public const string AllowedStatuses    = Prefix + ".MovementRequests.AllowedStatuses";     // İzin verilen talep durumları. Örnek değer: "1,2,3,4,5,6"
        public const string DefaultPriority    = Prefix + ".MovementRequests.DefaultPriority";     // Varsayılan talep önceliği. Örnek değer: "2" (Normal)
        public const string AllowedPriorities  = Prefix + ".MovementRequests.AllowedPriorities";   // İzin verilen talep öncelikleri. Örnek değer: "1,2,3,4"
    }

    /// <summary>
    /// Sevkiyat modülüne ait ShipmentStatusEnum setting anahtarları.
    /// </summary>
    public static class Shipments
    {
        public const string DefaultStatus   = Prefix + ".Shipments.DefaultStatus";    // Varsayılan sevkiyat durumu. Örnek değer: "1" (Preparing)
        public const string AllowedStatuses = Prefix + ".Shipments.AllowedStatuses";  // İzin verilen sevkiyat durumları. Örnek değer: "1,2,3,4"
        public const string AllowedShipmentStatuses = AllowedStatuses;                // Geriye dönük uyumluluk aliası
    }

    /// <summary>
    /// Stok hareketi modülüne ait StockMovementTypeEnum setting anahtarları.
    /// </summary>
    public static class StockMovements
    {
        public const string DefaultType   = Prefix + ".StockMovements.DefaultType";    // Varsayılan stok hareket tipi. Örnek değer: "1" (In)
        public const string AllowedTypes  = Prefix + ".StockMovements.AllowedTypes";   // İzin verilen stok hareket tipleri. Örnek değer: "1,2,3,4,5,6,7,8,9"
    }

    /// <summary>
    /// Manager ve SettingDefinitionProvider'ların kullandığı geriye dönük uyumlu erişim noktaları.
    /// Domain katmanındaki tüm manager'lar bu nested class'lar üzerinden setting anahtarlarına erişir.
    /// </summary>
    public static class Masters
    {
        public const string AllowedUnitTypes = Products.AllowedUnitTypes;
        public const string AllowedWorkerTypes = Workers.AllowedWorkerTypes;
        public const string AllowedSiteTypes = Sites.AllowedSiteTypes;
        public const string AllowedVehicleTypes = Vehicles.AllowedVehicleTypes;
    }

    /// <summary>
    /// Domain manager'ların kullandığı Movements erişim noktaları (MovementRequests'e yönlendirir).
    /// </summary>
    public static class Movements
    {
        public const string AllowedMovementPriorities = MovementRequests.AllowedPriorities;
        public const string AllowedMovementStatuses = MovementRequests.AllowedStatuses;
        public const string AllowedApprovalStatuses = Prefix + ".Movements.AllowedApprovalStatuses";
    }

    /// <summary>
    /// Domain manager'ların kullandığı Stock erişim noktaları (StockMovements'a yönlendirir).
    /// </summary>
    public static class Stock
    {
        public const string AllowedStockMovementTypes = StockMovements.AllowedTypes;
    }
}
