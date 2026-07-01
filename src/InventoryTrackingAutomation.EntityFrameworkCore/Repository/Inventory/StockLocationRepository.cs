using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Interface.Inventory;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace InventoryTrackingAutomation.Repository.Stock;

/// <summary>
/// StockLocation entity'si icin EF Core repository implementasyonu.
/// </summary>
public class StockLocationRepository : BaseRepository<StockLocation>, IStockLocationRepository
{
    public StockLocationRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async System.Threading.Tasks.Task<InventoryTrackingAutomation.Models.Inventory.ProductStockSummaryModel> GetProductStockSummaryAsync(System.Guid productId)
    {
        var dbContext = await GetDbContextAsync();
        
        var locations = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
            dbContext.StockLocations.Where(x => x.ProductId == productId));

        var vehicleIds = locations
            .Where(x => x.LocationType == InventoryTrackingAutomation.Enums.Inventory.StockLocationTypeEnum.Vehicle)
            .Select(x => (System.Guid?)x.LocationId)
            .ToList();

        var activeVehicleTasks = vehicleIds.Count > 0 
            ? await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
                dbContext.VehicleTasks.Where(x => vehicleIds.Contains(x.VehicleId) && !x.ReleasedAt.HasValue))
            : new System.Collections.Generic.List<InventoryTrackingAutomation.Entities.Tasks.VehicleTask>();

        var locationSummaries = locations
            .Select(location =>
            {
                var vehicleTask = location.LocationType == InventoryTrackingAutomation.Enums.Inventory.StockLocationTypeEnum.Vehicle
                    ? activeVehicleTasks.FirstOrDefault(x => x.VehicleId == location.LocationId)
                    : null;

                return new InventoryTrackingAutomation.Models.Inventory.ProductStockLocationSummaryModel
                {
                    LocationType = location.LocationType,
                    WarehouseId = location.LocationType == InventoryTrackingAutomation.Enums.Inventory.StockLocationTypeEnum.Warehouse ? location.LocationId : null,
                    VehicleId = location.LocationType == InventoryTrackingAutomation.Enums.Inventory.StockLocationTypeEnum.Vehicle ? location.LocationId : null,
                    VehicleTaskId = vehicleTask?.Id,
                    TaskId = vehicleTask?.TaskId,
                    Quantity = location.Quantity,
                    ReservedQuantity = location.ReservedQuantity
                };
            })
            .ToList();

        return new InventoryTrackingAutomation.Models.Inventory.ProductStockSummaryModel
        {
            ProductId = productId,
            TotalQuantity = locations.Sum(x => x.Quantity),
            WarehouseQuantity = locations
                .Where(x => x.LocationType == InventoryTrackingAutomation.Enums.Inventory.StockLocationTypeEnum.Warehouse)
                .Sum(x => x.Quantity),
            VehicleQuantity = locations
                .Where(x => x.LocationType == InventoryTrackingAutomation.Enums.Inventory.StockLocationTypeEnum.Vehicle)
                .Sum(x => x.Quantity),
            ActiveTaskQuantity = locationSummaries
                .Where(x => x.TaskId.HasValue)
                .Sum(x => x.Quantity),
            Locations = locationSummaries
        };
    }

    public async System.Threading.Tasks.Task<System.Collections.Generic.List<InventoryTrackingAutomation.Models.Inventory.VehicleInventoryModel>> GetVehicleInventoriesAsync(System.Guid vehicleId)
    {
        var dbContext = await GetDbContextAsync();
        
        var locations = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
            dbContext.StockLocations.Where(x => 
                x.LocationType == InventoryTrackingAutomation.Enums.Inventory.StockLocationTypeEnum.Vehicle && 
                x.LocationId == vehicleId));

        var activeVehicleTask = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
            dbContext.VehicleTasks.Where(x => x.VehicleId == vehicleId && !x.ReleasedAt.HasValue));

        return locations
            .Select(location => new InventoryTrackingAutomation.Models.Inventory.VehicleInventoryModel
            {
                VehicleId = vehicleId,
                ProductId = location.ProductId,
                VehicleTaskId = activeVehicleTask?.Id,
                TaskId = activeVehicleTask?.TaskId,
                Quantity = location.Quantity,
                ReservedQuantity = location.ReservedQuantity
            })
            .ToList();
    }

    public async System.Threading.Tasks.Task<System.Collections.Generic.List<InventoryTrackingAutomation.Models.Tasks.TaskInventoryModel>> GetTaskInventoryAsync(System.Guid inventoryTaskId)
    {
        var dbContext = await GetDbContextAsync();
        
        var vehicleTasks = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
            dbContext.VehicleTasks.Where(x => x.TaskId == inventoryTaskId && !x.ReleasedAt.HasValue));
            
        var vehicleIds = vehicleTasks.Select(x => x.VehicleId).Distinct().ToList();
        
        var locations = vehicleIds.Count > 0 
            ? await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
                dbContext.StockLocations.Where(x => 
                    x.LocationType == InventoryTrackingAutomation.Enums.Inventory.StockLocationTypeEnum.Vehicle && 
                    vehicleIds.Contains(x.LocationId)))
            : new System.Collections.Generic.List<InventoryTrackingAutomation.Entities.Inventory.StockLocation>();

        return locations
            .Select(location =>
            {
                var vehicleTask = vehicleTasks.First(x => x.VehicleId == location.LocationId);
                return new InventoryTrackingAutomation.Models.Tasks.TaskInventoryModel
                {
                    TaskId = inventoryTaskId,
                    VehicleTaskId = vehicleTask.Id,
                    VehicleId = vehicleTask.VehicleId,
                    ProductId = location.ProductId,
                    Quantity = location.Quantity,
                    ReservedQuantity = location.ReservedQuantity
                };
            })
            .ToList();
    }
}
