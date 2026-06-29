using System;

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
    // Attribute cache ve invalidation ayni key formatini kullansin diye sablonlar merkezi tutulur.
    public const string ProductStockSummaryTemplate = "stock-summary:{id}";
    public const string VehicleInventoriesTemplate = "vehicle-inventories:{id}";
    public const string TaskInventoryTemplate = "task-inventory:{id}";
    public const string TaskVehiclesTemplate = "task-vehicles:{id}";

    public static string ProductStockSummary(Guid productId) => Format(ProductStockSummaryTemplate, productId);
    public static string VehicleInventories(Guid vehicleId) => Format(VehicleInventoriesTemplate, vehicleId);
    public static string TaskInventory(Guid taskId) => Format(TaskInventoryTemplate, taskId);
    public static string TaskVehicles(Guid taskId) => Format(TaskVehiclesTemplate, taskId);

    private static string Format(string template, Guid id)
    {
        return template.Replace("{id}", id.ToString("D"), StringComparison.Ordinal);
    }
}
