using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Interface.Stock;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Managers.Masters;
using InventoryTrackingAutomation.Managers.Tasks;
using InventoryTrackingAutomation.Models.Stock;
using InventoryTrackingAutomation.Models.Tasks;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Stock;

/// <summary>
/// PITON stok gorunurlugu icin urun, arac ve gorev bazli okuma kurallarini yonetir.
/// </summary>
public class InventoryQueryManager : ITransientDependency
{
    private readonly IStockLocationRepository _stockLocationRepository;
    private readonly IVehicleTaskRepository _vehicleTaskRepository;
    private readonly ProductManager _productManager;
    private readonly VehicleManager _vehicleManager;
    private readonly InventoryTaskManager _inventoryTaskManager;

    public InventoryQueryManager(
        IStockLocationRepository stockLocationRepository,
        IVehicleTaskRepository vehicleTaskRepository,
        ProductManager productManager,
        VehicleManager vehicleManager,
        InventoryTaskManager inventoryTaskManager)
    {
        _stockLocationRepository = stockLocationRepository;
        _vehicleTaskRepository = vehicleTaskRepository;
        _productManager = productManager;
        _vehicleManager = vehicleManager;
        _inventoryTaskManager = inventoryTaskManager;
    }

    public async Task<ProductStockSummaryModel> GetProductStockSummaryAsync(Guid productId)
    {
        await _productManager.EnsureExistsAsync(productId);

        var locations = await _stockLocationRepository.GetListAsync(x => x.ProductId == productId);
        var activeVehicleTasks = await GetActiveVehicleTasksAsync(locations.Select(x => x.VehicleId));

        var locationSummaries = locations
            .Select(location => CreateLocationSummary(location, activeVehicleTasks))
            .ToList();

        return new ProductStockSummaryModel
        {
            ProductId = productId,
            TotalQuantity = locations.Sum(x => x.Quantity),
            WarehouseQuantity = locations
                .Where(x => x.LocationType == InventoryLocationTypeEnum.Warehouse)
                .Sum(x => x.Quantity),
            VehicleQuantity = locations
                .Where(x => x.LocationType == InventoryLocationTypeEnum.Vehicle)
                .Sum(x => x.Quantity),
            ActiveTaskQuantity = locationSummaries
                .Where(x => x.InventoryTaskId.HasValue)
                .Sum(x => x.Quantity),
            Locations = locationSummaries
        };
    }

    public async Task<List<VehicleInventoryModel>> GetVehicleInventoriesAsync(Guid vehicleId)
    {
        await _vehicleManager.EnsureExistsAsync(vehicleId);

        var locations = await _stockLocationRepository.GetListAsync(x =>
            x.LocationType == InventoryLocationTypeEnum.Vehicle &&
            x.VehicleId == vehicleId);
        var activeVehicleTask = (await _vehicleTaskRepository.GetListAsync(x =>
                x.VehicleId == vehicleId &&
                x.IsActive))
            .FirstOrDefault();

        return locations
            .Select(location => new VehicleInventoryModel
            {
                VehicleId = vehicleId,
                ProductId = location.ProductId,
                VehicleTaskId = activeVehicleTask?.Id,
                InventoryTaskId = activeVehicleTask?.InventoryTaskId,
                Quantity = location.Quantity,
                ReservedQuantity = location.ReservedQuantity
            })
            .ToList();
    }

    public async Task<List<TaskVehicleModel>> GetTaskVehiclesAsync(Guid inventoryTaskId)
    {
        await _inventoryTaskManager.EnsureExistsAsync(inventoryTaskId);

        var vehicleTasks = await _vehicleTaskRepository.GetListAsync(x => x.InventoryTaskId == inventoryTaskId);
        return vehicleTasks
            .Select(x => new TaskVehicleModel
            {
                VehicleTaskId = x.Id,
                InventoryTaskId = x.InventoryTaskId,
                VehicleId = x.VehicleId,
                AssignedAt = x.AssignedAt,
                ReleasedAt = x.ReleasedAt,
                IsActive = x.IsActive
            })
            .ToList();
    }

    public async Task<List<TaskInventoryModel>> GetTaskInventoryAsync(Guid inventoryTaskId)
    {
        await _inventoryTaskManager.EnsureExistsAsync(inventoryTaskId);

        var vehicleTasks = await _vehicleTaskRepository.GetListAsync(x =>
            x.InventoryTaskId == inventoryTaskId &&
            x.IsActive);
        var vehicleIds = vehicleTasks.Select(x => x.VehicleId).Distinct().ToList();
        var locations = await _stockLocationRepository.GetListAsync(x =>
            x.LocationType == InventoryLocationTypeEnum.Vehicle &&
            x.VehicleId.HasValue &&
            vehicleIds.Contains(x.VehicleId.Value));

        return locations
            .Select(location =>
            {
                var vehicleTask = vehicleTasks.First(x => x.VehicleId == location.VehicleId);
                return new TaskInventoryModel
                {
                    InventoryTaskId = inventoryTaskId,
                    VehicleTaskId = vehicleTask.Id,
                    VehicleId = vehicleTask.VehicleId,
                    ProductId = location.ProductId,
                    Quantity = location.Quantity,
                    ReservedQuantity = location.ReservedQuantity
                };
            })
            .ToList();
    }

    private async Task<List<Entities.Tasks.VehicleTask>> GetActiveVehicleTasksAsync(IEnumerable<Guid?> vehicleIds)
    {
        var ids = vehicleIds
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .Distinct()
            .ToList();

        if (ids.Count == 0)
        {
            return new List<Entities.Tasks.VehicleTask>();
        }

        return await _vehicleTaskRepository.GetListAsync(x => ids.Contains(x.VehicleId) && x.IsActive);
    }

    private static ProductStockLocationSummaryModel CreateLocationSummary(
        Entities.Stock.StockLocation location,
        IReadOnlyCollection<Entities.Tasks.VehicleTask> activeVehicleTasks)
    {
        // Arac lokasyonlarinda aktif gorev baglami gorunurluge eklenir.
        var vehicleTask = location.VehicleId.HasValue
            ? activeVehicleTasks.FirstOrDefault(x => x.VehicleId == location.VehicleId.Value)
            : null;

        return new ProductStockLocationSummaryModel
        {
            LocationType = location.LocationType,
            WarehouseSiteId = location.WarehouseSiteId,
            VehicleId = location.VehicleId,
            VehicleTaskId = vehicleTask?.Id,
            InventoryTaskId = vehicleTask?.InventoryTaskId,
            Quantity = location.Quantity,
            ReservedQuantity = location.ReservedQuantity
        };
    }
}
