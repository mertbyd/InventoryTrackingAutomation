using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Entities.Stock;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Interface.Stock;
using InventoryTrackingAutomation.Interface.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace InventoryTrackingAutomation.Managers.Stock;

/// <summary>
/// Hareket talebi stok kurallarini yoneten domain manager.
/// </summary>
//işlevi: Hareket talebi onaylandığında stok düşümü, artırımı ve ledger (transaction) kayıtlarını yönetir.
//sistemdeki görevii: Onaylanmış transferlerin stok üzerindeki etkilerini uygular. PITON planı uyarınca StockTransferManager ile değiştirilecek ve silinecektir.
public class MovementRequestStockManager : DomainService
{
    private readonly IStockLocationRepository _stockLocationRepository;
    private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
    private readonly IVehicleTaskRepository _vehicleTaskRepository;

    public MovementRequestStockManager(
        IStockLocationRepository stockLocationRepository,
        IInventoryTransactionRepository inventoryTransactionRepository,
        IVehicleTaskRepository vehicleTaskRepository)
    {
        _stockLocationRepository = stockLocationRepository;
        _inventoryTransactionRepository = inventoryTransactionRepository;
        _vehicleTaskRepository = vehicleTaskRepository;
    }

    public async Task ApplyApprovedTransferAsync(MovementRequest request, IReadOnlyList<MovementRequestLine> lines)
    {
        var activeVehicleTaskId = await ResolveActiveVehicleTaskIdAsync(request);

        foreach (var line in lines)
        {
            await DecreaseSourceStockLocationAsync(request, line);
            await IncreaseTargetStockLocationAsync(request, line);
            await CreateInventoryTransactionAsync(request, line, activeVehicleTaskId);
        }
    }

    private async Task DecreaseSourceStockLocationAsync(MovementRequest request, MovementRequestLine line)
    {
        // Onaylanan hareket sadece yeni polymorphic stok lokasyonundan dusulur.
        var sourceLocation = await _stockLocationRepository.FindAsync(x =>
            x.ProductId == line.ProductId &&
            x.LocationType == StockLocationTypeEnum.Warehouse &&
            x.LocationId == request.SourceWarehouseId);

        if (sourceLocation == null || sourceLocation.Quantity < line.Quantity)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.StockLocations.InsufficientStock)
                .WithData("ProductId", line.ProductId)
                .WithData("RequestedQuantity", line.Quantity)
                .WithData("AvailableQuantity", sourceLocation?.Quantity ?? 0);
        }

        sourceLocation.Quantity -= line.Quantity;
        await _stockLocationRepository.UpdateAsync(sourceLocation, autoSave: true);
    }

    private async Task IncreaseTargetStockLocationAsync(MovementRequest request, MovementRequestLine line)
    {
        var targetLocationType = ResolveTargetLocationType(request);
        var targetLocation = await FindTargetStockLocationAsync(request, line, targetLocationType);

        if (targetLocation == null)
        {
            targetLocation = CreateTargetStockLocation(request, line, targetLocationType);
            await _stockLocationRepository.InsertAsync(targetLocation, autoSave: true);
            return;
        }

        targetLocation.Quantity += line.Quantity;
        await _stockLocationRepository.UpdateAsync(targetLocation, autoSave: true);
    }

    private async Task<StockLocation?> FindTargetStockLocationAsync(
        MovementRequest request,
        MovementRequestLine line,
        StockLocationTypeEnum targetLocationType)
    {
        if (targetLocationType == StockLocationTypeEnum.Vehicle)
        {
            return await _stockLocationRepository.FindAsync(x =>
                x.ProductId == line.ProductId &&
                x.LocationType == StockLocationTypeEnum.Vehicle &&
                x.LocationId == request.RequestedVehicleId);
        }

        return await _stockLocationRepository.FindAsync(x =>
            x.ProductId == line.ProductId &&
            x.LocationType == StockLocationTypeEnum.Warehouse &&
            x.LocationId == request.TargetWarehouseId);
    }

    private StockLocation CreateTargetStockLocation(
        MovementRequest request,
        MovementRequestLine line,
        StockLocationTypeEnum targetLocationType)
    {
        // Hedef lokasyon arac secimine gore arac veya depo olarak olusturulur.
        return new StockLocation(GuidGenerator.Create())
        {
            ProductId = line.ProductId,
            LocationType = targetLocationType,
            LocationId = targetLocationType == StockLocationTypeEnum.Warehouse
                ? request.TargetWarehouseId!.Value
                : request.RequestedVehicleId!.Value,
            Quantity = line.Quantity,
            ReservedQuantity = 0
        };
    }

    private async Task CreateInventoryTransactionAsync(
        MovementRequest request,
        MovementRequestLine line,
        System.Guid? activeVehicleTaskId)
    {
        var targetLocationType = ResolveTargetLocationType(request);
        var transaction = new InventoryTransaction(GuidGenerator.Create())
        {
            ProductId = line.ProductId,
            TransactionType = ResolveTransactionType(targetLocationType),
            Quantity = line.Quantity,
            SourceLocationType = StockLocationTypeEnum.Warehouse,
            SourceLocationId = request.SourceWarehouseId,
            TargetLocationType = targetLocationType,
            TargetLocationId = targetLocationType == StockLocationTypeEnum.Warehouse
                ? request.TargetWarehouseId
                : request.RequestedVehicleId,
            RelatedMovementRequestId = request.Id,
            RelatedTaskId = activeVehicleTaskId,
            OccurredAt = Clock.Now,
            Note = request.RequestNumber
        };

        await _inventoryTransactionRepository.InsertAsync(transaction, autoSave: true);
    }

    private async Task<System.Guid?> ResolveActiveVehicleTaskIdAsync(MovementRequest request)
    {
        if (!request.RequestedVehicleId.HasValue)
        {
            return null;
        }

        var activeVehicleTask = (await _vehicleTaskRepository.GetListAsync(x =>
                x.VehicleId == request.RequestedVehicleId.Value &&
                x.IsActive))
            .FirstOrDefault();

        if (activeVehicleTask == null)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.InventoryTransactions.InvalidTransfer)
                .WithData("VehicleId", request.RequestedVehicleId.Value);
        }

        return activeVehicleTask.Id;
    }

    private static StockLocationTypeEnum ResolveTargetLocationType(MovementRequest request)
    {
        // Arac secildiyse malzeme gorevdeki araca, aksi halde hedef depoya gider.
        return request.RequestedVehicleId.HasValue
            ? StockLocationTypeEnum.Vehicle
            : StockLocationTypeEnum.Warehouse;
    }

    private static InventoryTransactionTypeEnum ResolveTransactionType(StockLocationTypeEnum targetLocationType)
    {
        return targetLocationType == StockLocationTypeEnum.Vehicle
            ? InventoryTransactionTypeEnum.WarehouseToVehicle
            : InventoryTransactionTypeEnum.WarehouseToWarehouse;
    }
}
