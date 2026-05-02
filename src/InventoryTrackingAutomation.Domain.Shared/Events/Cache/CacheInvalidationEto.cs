namespace InventoryTrackingAutomation.Events.Cache;

/// <summary>
/// Herhangi bir cache key'ini geçersiz kılmak için fırlatılan genel amaçlı event.
/// Hangi key'lerin temizleneceği çağıran tarafından CacheKeys helper ile belirlenir.
/// </summary>
public class CacheInvalidationEto
{
    public string[] Keys { get; set; } = [];

    public static CacheInvalidationEto ForKeys(params string[] keys) => new() { Keys = keys };
}

/// <summary>
/// Sistemde kullanılan cache key şablonlarını merkezi olarak üretir.
/// </summary>
public static class CacheKeys
{
    public static string ProductStockSummary(Guid productId)   => $"stock-summary:{productId}";
    public static string VehicleInventories(Guid vehicleId)    => $"vehicle-inventories:{vehicleId}";
    public static string TaskInventory(Guid taskId)            => $"task-inventory:{taskId}";
    public static string TaskVehicles(Guid taskId)             => $"task-vehicles:{taskId}";
}
