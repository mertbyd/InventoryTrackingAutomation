using InventoryTrackingAutomation.Models.Inventory;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using InventoryTrackingAutomation.Dtos.Inventory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public async Task<ProductStockSummaryModel> GetProductStockSummaryAsync(Guid productId)
    {
        var dbContext = await GetDbContextAsync();
        
        var locations = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
            dbContext.StockLocations.Where(x => x.ProductId == productId));

        var vehicleIds = locations
            .Where(x => x.LocationType == StockLocationTypeEnum.Vehicle)
            .Select(x => (Guid?)x.LocationId)
            .ToList();

        var activeVehicleTasks = vehicleIds.Count > 0 
            ? await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
                dbContext.VehicleTasks.Where(x => vehicleIds.Contains(x.VehicleId) && !x.ReleasedAt.HasValue))
            : new List<VehicleTask>();

        var locationSummaries = locations
            .Select(location =>
            {
                var vehicleTask = location.LocationType == StockLocationTypeEnum.Vehicle
                    ? activeVehicleTasks.FirstOrDefault(x => x.VehicleId == location.LocationId)
                    : null;

                return new ProductStockLocationSummaryModel
                {
                    LocationType = location.LocationType,
                    WarehouseId = location.LocationType == StockLocationTypeEnum.Warehouse ? location.LocationId : null,
                    VehicleId = location.LocationType == StockLocationTypeEnum.Vehicle ? location.LocationId : null,
                    VehicleTaskId = vehicleTask?.Id,
                    TaskId = vehicleTask?.TaskId,
                    Quantity = location.Quantity,
                    ReservedQuantity = location.ReservedQuantity
                };
            })
            .ToList();

        return new ProductStockSummaryModel
        {
            ProductId = productId,
            TotalQuantity = locations.Sum(x => x.Quantity),
            WarehouseQuantity = locations
                .Where(x => x.LocationType == StockLocationTypeEnum.Warehouse)
                .Sum(x => x.Quantity),
            VehicleQuantity = locations
                .Where(x => x.LocationType == StockLocationTypeEnum.Vehicle)
                .Sum(x => x.Quantity),
            ActiveTaskQuantity = locationSummaries
                .Where(x => x.TaskId.HasValue)
                .Sum(x => x.Quantity),
            Locations = locationSummaries
        };
    }

    public async Task<List<VehicleInventoryModel>> GetVehicleInventoriesAsync(Guid vehicleId)
    {
        var dbContext = await GetDbContextAsync();
        
        var locations = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
            dbContext.StockLocations.Where(x => 
                x.LocationType == StockLocationTypeEnum.Vehicle && 
                x.LocationId == vehicleId));

        var activeVehicleTask = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
            dbContext.VehicleTasks.Where(x => x.VehicleId == vehicleId && !x.ReleasedAt.HasValue));

        return locations
            .Select(location => new VehicleInventoryModel
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

    public async Task<List<TaskInventoryModel>> GetTaskInventoryAsync(Guid inventoryTaskId)
    {
        var dbContext = await GetDbContextAsync();
        
        var vehicleTasks = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
            dbContext.VehicleTasks.Where(x => x.TaskId == inventoryTaskId && !x.ReleasedAt.HasValue));
            
        var vehicleIds = vehicleTasks.Select(x => x.VehicleId).Distinct().ToList();
        
        var locations = vehicleIds.Count > 0 
            ? await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
                dbContext.StockLocations.Where(x => 
                    x.LocationType == StockLocationTypeEnum.Vehicle && 
                    vehicleIds.Contains(x.LocationId)))
            : new List<StockLocation>();

        return locations
            .Select(location =>
            {
                var vehicleTask = vehicleTasks.First(x => x.VehicleId == location.LocationId);
                return new TaskInventoryModel
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

    public override async Task<IQueryable<StockLocation>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).Include(x => x.Product).Include(x => x.Warehouse);
    }

    public async Task<List<InventoryGridItemDto>> GetInventoryGridListAsync()
    {
        var dbContext = await GetDbContextAsync();
        
        return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
            dbContext.StockLocations
                .Where(x => x.LocationType == StockLocationTypeEnum.Warehouse)
                .Select(x => new InventoryGridItemDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    Name = x.Product != null ? x.Product.Name : null,
                    CategoryId = x.Product != null && x.Product.CategoryId.HasValue ? x.Product.CategoryId.Value : Guid.Empty,
                    CategoryName = x.Product != null && x.Product.Category != null ? x.Product.Category.Name : null,
                    WarehouseId = x.Warehouse != null ? x.Warehouse.Id : Guid.Empty,
                    WarehouseName = x.Warehouse != null ? x.Warehouse.Name : null,
                    WarehouseLocation = x.Warehouse != null ? x.Warehouse.Code : null,
                    Quantity = x.Quantity
                })
        );
    }
}



