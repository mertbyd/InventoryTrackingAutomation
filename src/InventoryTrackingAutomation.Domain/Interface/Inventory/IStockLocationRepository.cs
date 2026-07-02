using InventoryTrackingAutomation.Entities.Inventory;

namespace InventoryTrackingAutomation.Interface.Inventory;

/// <summary>
/// StockLocation entity'si icin repository arayuzu.
/// </summary>
public interface IStockLocationRepository : IBaseRepository<StockLocation>
{
    System.Threading.Tasks.Task<InventoryTrackingAutomation.Models.Inventory.ProductStockSummaryModel> GetProductStockSummaryAsync(System.Guid productId);
    System.Threading.Tasks.Task<System.Collections.Generic.List<InventoryTrackingAutomation.Models.Inventory.VehicleInventoryModel>> GetVehicleInventoriesAsync(System.Guid vehicleId);
    System.Threading.Tasks.Task<System.Collections.Generic.List<InventoryTrackingAutomation.Models.Tasks.TaskInventoryModel>> GetTaskInventoryAsync(System.Guid inventoryTaskId);
    System.Threading.Tasks.Task<System.Collections.Generic.List<InventoryTrackingAutomation.Dtos.Inventory.InventoryGridItemDto>> GetInventoryGridListAsync();
}

