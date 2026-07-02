using InventoryTrackingAutomation.Models.Inventory;
using InventoryTrackingAutomation.Models.Tasks;
using InventoryTrackingAutomation.Dtos.Inventory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Inventory;

namespace InventoryTrackingAutomation.Interface.Inventory;

/// <summary>
/// StockLocation entity'si icin repository arayuzu.
/// </summary>
public interface IStockLocationRepository : IBaseRepository<StockLocation>
{
    Task<ProductStockSummaryModel> GetProductStockSummaryAsync(Guid productId);
    Task<List<VehicleInventoryModel>> GetVehicleInventoriesAsync(Guid vehicleId);
    Task<List<TaskInventoryModel>> GetTaskInventoryAsync(Guid inventoryTaskId);
    Task<List<InventoryGridItemDto>> GetInventoryGridListAsync();
}


