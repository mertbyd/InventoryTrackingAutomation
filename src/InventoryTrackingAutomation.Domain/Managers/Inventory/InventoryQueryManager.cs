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
        return await _stockLocationRepository.GetProductStockSummaryAsync(productId);
    }

    /// Araç envanterlerini getirmek için kullanılır.
    public async Task<List<VehicleInventoryModel>> GetVehicleInventoriesAsync(Guid vehicleId)
    {
        await _vehicleManager.EnsureExistsAsync(vehicleId);
        return await _stockLocationRepository.GetVehicleInventoriesAsync(vehicleId);
    }

    /// Görev araçlarını getirmek için kullanılır.
    public async Task<List<TaskVehicleModel>> GetTaskVehiclesAsync(Guid inventoryTaskId)
    {
        await _inventoryTaskManager.EnsureExistsAsync(inventoryTaskId);
        return await _vehicleTaskRepository.GetTaskVehiclesByTaskIdAsync(inventoryTaskId);
    }

    /// Görev envanterini getirmek için kullanılır.
    public async Task<List<TaskInventoryModel>> GetTaskInventoryAsync(Guid inventoryTaskId)
    {
        await _inventoryTaskManager.EnsureExistsAsync(inventoryTaskId);
        return await _stockLocationRepository.GetTaskInventoryAsync(inventoryTaskId);
    }
}
