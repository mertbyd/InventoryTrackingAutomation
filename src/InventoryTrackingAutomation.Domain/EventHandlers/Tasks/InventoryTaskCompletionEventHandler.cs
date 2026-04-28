using System;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Events.Tasks;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Interface.Inventory;
using InventoryTrackingAutomation.Managers.Inventory;
using InventoryTrackingAutomation.Managers.Tasks;
using InventoryTrackingAutomation.Models.Inventory;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.Uow;
using Volo.Abp.Domain.Repositories;

namespace InventoryTrackingAutomation.EventHandlers.Tasks;

/// <summary>
/// Task Completed/Cancelled olunca atanmis araclari serbest birakir
/// ve uzerlerindeki tum stoku iade hedefi depoya geri tasir (atomik).
/// </summary>
//işlevi: Görev tamamlandığında veya iptal edildiğinde araçlardaki kalan ürünleri depoya iade eder ve araçları serbest bırakır.
//sistemdeki görevi: Görev yaşam döngüsünün sonunda stokları ve kaynakları temizleyerek sistemi tutarlı hale getirir.
public class InventoryTaskCompletionEventHandler :
    ILocalEventHandler<InventoryTaskStatusChangedEto>,
    ITransientDependency
{
    private readonly IInventoryTaskRepository _taskRepository;
    private readonly IVehicleTaskRepository _vehicleTaskRepository;
    private readonly IStockLocationRepository _stockLocationRepository;
    private readonly StockTransferManager _stockTransferManager;
    private readonly VehicleTaskManager _vehicleTaskManager;
    private readonly IInventoryTransactionRepository _inventoryTransactionRepository;

    public InventoryTaskCompletionEventHandler(
        IInventoryTaskRepository taskRepository,
        IVehicleTaskRepository vehicleTaskRepository,
        IStockLocationRepository stockLocationRepository,
        StockTransferManager stockTransferManager,
        VehicleTaskManager vehicleTaskManager,
        IInventoryTransactionRepository inventoryTransactionRepository)
    {
        _taskRepository = taskRepository;
        _vehicleTaskRepository = vehicleTaskRepository;
        _stockLocationRepository = stockLocationRepository;
        _stockTransferManager = stockTransferManager;
        _vehicleTaskManager = vehicleTaskManager;
        _inventoryTransactionRepository = inventoryTransactionRepository;
    }

    [UnitOfWork]
    public virtual async Task HandleEventAsync(InventoryTaskStatusChangedEto e)
    {
        // Sadece terminal statulerde tetiklenir.
        if (e.NewStatus != TaskStatusEnum.Completed && e.NewStatus != TaskStatusEnum.Cancelled)
            return;

        var task = await _taskRepository.FindAsync(e.TaskId);
        if (task == null) return;

        // 1. Aktif arac atamalarini bul.
        var activeAssignments = await _vehicleTaskRepository.GetListAsync(
            x => x.InventoryTaskId == e.TaskId && x.IsActive);

        foreach (var assignment in activeAssignments)
        {
            // 2. Aractaki tum stoku bul ve iade hedefine geri tasi.
            var vehicleStocks = await _stockLocationRepository.GetListAsync(x => x.LocationType == StockLocationTypeEnum.Vehicle && x.LocationId == assignment.VehicleId);

            var returnWarehouseId = task.ReturnWarehouseId.HasValue 
                ? task.ReturnWarehouseId.Value 
                : await ResolveLastSourceWarehouseAsync(assignment.VehicleId);
            
            if (returnWarehouseId == Guid.Empty) continue;

            foreach (var stock in vehicleStocks)
            {
                if (stock.Quantity <= 0) continue;

                // Atomik geri tasima: aractan dus, depoya ekle, ledger yaz.
                await _stockTransferManager.ExecuteAsync(new StockTransferModel
                {
                    ProductId = stock.ProductId,
                    Quantity = stock.Quantity,
                    SourceLocationType = StockLocationTypeEnum.Vehicle,
                    SourceLocationId = assignment.VehicleId,
                    DestinationLocationType = StockLocationTypeEnum.Warehouse,
                    DestinationLocationId = returnWarehouseId,
                    TransactionType = InventoryTransactionTypeEnum.VehicleToWarehouse,
                    RelatedTaskId = task.Id,
                    Note = $"Auto-return on task {task.Status}"
                });
            }
        }

        // 3. Tum aktif atamalari sonlandir.
        await _vehicleTaskManager.ReleaseAllForTaskAsync(e.TaskId);
    }

    // Iade hedefi belirsizse aracin son WarehouseToVehicle transaction'inin kaynagina iade.
    private async Task<Guid> ResolveLastSourceWarehouseAsync(Guid vehicleId)
    {
        var lastTransaction = (await _inventoryTransactionRepository.GetListAsync(
            x => x.TargetLocationType == StockLocationTypeEnum.Vehicle && x.TargetLocationId == vehicleId && x.TransactionType == InventoryTransactionTypeEnum.WarehouseToVehicle
        )).OrderByDescending(x => x.OccurredAt).FirstOrDefault();

        if (lastTransaction != null && lastTransaction.SourceLocationType == StockLocationTypeEnum.Warehouse)
        {
            return lastTransaction.SourceLocationId!.Value;
        }

        throw new Volo.Abp.BusinessException(InventoryTrackingAutomationErrorCodes.General.InvalidOperation)
            .WithData("VehicleId", vehicleId)
            .WithData("Reason", "Cannot resolve return warehouse");
    }
}
