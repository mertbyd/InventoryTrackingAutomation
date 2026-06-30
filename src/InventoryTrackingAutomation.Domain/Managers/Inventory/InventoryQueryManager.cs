using System;
using InventoryTrackingAutomation.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Interface.Inventory;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Managers.Masters;
using InventoryTrackingAutomation.Managers.Tasks;
using InventoryTrackingAutomation.Models.Inventory;
using InventoryTrackingAutomation.Models.Tasks;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Inventory;

/// <summary>
/// PITON stok gorunurlugu icin urun, arac ve gorev bazli okuma kurallarini yonetir.
/// </summary>
//işlevi: InventoryQuery etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class InventoryQueryManager : InventoryTrackingAutomationLazyService, ITransientDependency
{
    public InventoryQueryManager(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IStockLocationRepository _stockLocationRepository => LazyGetRequiredService<IStockLocationRepository>();
    private IVehicleTaskRepository _vehicleTaskRepository => LazyGetRequiredService<IVehicleTaskRepository>();
    private ProductManager _productManager => LazyGetRequiredService<ProductManager>();
    private VehicleManager _vehicleManager => LazyGetRequiredService<VehicleManager>();
    private InventoryTaskManager _inventoryTaskManager => LazyGetRequiredService<InventoryTaskManager>();



    /// Ürün stok özetini getirmek için kullanılır.
    public async Task<ProductStockSummaryModel> GetProductStockSummaryAsync(Guid productId)
    {
        await _productManager.EnsureExistsAsync(productId);

        var locations = await _stockLocationRepository.GetListAsync(x => x.ProductId == productId);
        var activeVehicleTasks = await GetActiveVehicleTasksAsync(locations
            .Where(x => x.LocationType == StockLocationTypeEnum.Vehicle)
            .Select(x => (Guid?)x.LocationId));

        var locationSummaries = locations
            .Select(location => CreateLocationSummary(location, activeVehicleTasks))
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

    /// Araç envanterlerini getirmek için kullanılır.
    public async Task<List<VehicleInventoryModel>> GetVehicleInventoriesAsync(Guid vehicleId)
    {
        await _vehicleManager.EnsureExistsAsync(vehicleId);

        var locations = await _stockLocationRepository.GetListAsync(x =>
            x.LocationType == StockLocationTypeEnum.Vehicle &&
            x.LocationId == vehicleId);
        var activeVehicleTask = (await _vehicleTaskRepository.GetListAsync(x =>
                x.VehicleId == vehicleId &&
                !x.ReleasedAt.HasValue))
            .FirstOrDefault();

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

    /// Görev araçlarını getirmek için kullanılır.
    public async Task<List<TaskVehicleModel>> GetTaskVehiclesAsync(Guid inventoryTaskId)
    {
        await _inventoryTaskManager.EnsureExistsAsync(inventoryTaskId);

        var vehicleTasks = await _vehicleTaskRepository.GetListAsync(x => x.TaskId == inventoryTaskId);
        return vehicleTasks
            .Select(x => new TaskVehicleModel
            {
                VehicleTaskId = x.Id,
                TaskId = x.TaskId,
                VehicleId = x.VehicleId,
                AssignedAt = x.AssignedAt,
                ReleasedAt = x.ReleasedAt
            })
            .ToList();
    }

    /// Görev envanterini getirmek için kullanılır.
    public async Task<List<TaskInventoryModel>> GetTaskInventoryAsync(Guid inventoryTaskId)
    {
        await _inventoryTaskManager.EnsureExistsAsync(inventoryTaskId);

        var vehicleTasks = await _vehicleTaskRepository.GetListAsync(x =>
            x.TaskId == inventoryTaskId &&
            !x.ReleasedAt.HasValue);
        var vehicleIds = vehicleTasks.Select(x => x.VehicleId).Distinct().ToList();
        var locations = await _stockLocationRepository.GetListAsync(x =>
            x.LocationType == StockLocationTypeEnum.Vehicle &&
            vehicleIds.Contains(x.LocationId));

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

    /// Aktif araç görevlerini getirmek için kullanılır.
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

        return await _vehicleTaskRepository.GetListAsync(x => ids.Contains(x.VehicleId) && !x.ReleasedAt.HasValue);
    }

    /// Lokasyon özeti oluşturmak için kullanılır.
    private static ProductStockLocationSummaryModel CreateLocationSummary(
        Entities.Inventory.StockLocation location,
        IReadOnlyCollection<Entities.Tasks.VehicleTask> activeVehicleTasks)
    {
        // Arac lokasyonlarinda aktif gorev baglami gorunurluge eklenir.
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
    }
}
