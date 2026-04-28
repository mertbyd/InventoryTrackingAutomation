using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Entities.Stock;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Interface.Stock;
using InventoryTrackingAutomation.Interface.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace InventoryTrackingAutomation.Managers.Stock;

/// <summary>
/// Hareket talebi stok kurallarini yoneten domain manager.
/// </summary>
public class MovementRequestStockManager : DomainService
{
    private readonly IProductStockRepository _productStockRepository;
    private readonly IStockLocationRepository _stockLocationRepository;
    private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
    private readonly IVehicleTaskRepository _vehicleTaskRepository;

    public MovementRequestStockManager(
        IProductStockRepository productStockRepository,
        IStockLocationRepository stockLocationRepository,
        IInventoryTransactionRepository inventoryTransactionRepository,
        IVehicleTaskRepository vehicleTaskRepository)
    {
        _productStockRepository = productStockRepository;
        _stockLocationRepository = stockLocationRepository;
        _inventoryTransactionRepository = inventoryTransactionRepository;
        _vehicleTaskRepository = vehicleTaskRepository;
    }

    public async Task ApplyApprovedTransferAsync(MovementRequest request, IReadOnlyList<MovementRequestLine> lines)
    {
        var activeVehicleTaskId = await ResolveActiveVehicleTaskIdAsync(request);

        foreach (var line in lines)
        {
            var remainingSourceQuantity = await DecreaseLegacySourceStockAsync(request, line);
            await SyncSourceWarehouseStockLocationAsync(request, line, remainingSourceQuantity);
            await IncreaseTargetStockLocationAsync(request, line);
            await CreateInventoryTransactionAsync(request, line, activeVehicleTaskId);
        }
    }

    private async Task<int> DecreaseLegacySourceStockAsync(MovementRequest request, MovementRequestLine line)
    {
        // Eski ProductStock tablosu simdilik mevcut stok dogrulamasinin kaynagi olmaya devam eder.
        var stock = await _productStockRepository.FindAsync(x =>
            x.ProductId == line.ProductId &&
            x.SiteId == request.SourceSiteId);

        if (stock == null || stock.TotalQuantity < line.Quantity)
        {
            // Yetersiz stokta exception firlatilir; UnitOfWork tum onay islemini geri alir.
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.ProductStocks.InsufficientStock)
                .WithData("ProductId", line.ProductId)
                .WithData("RequestedQuantity", line.Quantity)
                .WithData("AvailableQuantity", stock?.TotalQuantity ?? 0);
        }

        stock.TotalQuantity -= line.Quantity;
        await _productStockRepository.UpdateAsync(stock, autoSave: true);
        return stock.TotalQuantity;
    }

    private async Task SyncSourceWarehouseStockLocationAsync(
        MovementRequest request,
        MovementRequestLine line,
        int remainingSourceQuantity)
    {
        // Yeni PITON stok lokasyonu eski kaynak stok ile senkron tutulur.
        var sourceLocation = await _stockLocationRepository.FindAsync(x =>
            x.ProductId == line.ProductId &&
            x.LocationType == InventoryLocationTypeEnum.Warehouse &&
            x.LocationId == request.SourceSiteId);

        if (sourceLocation == null)
        {
            sourceLocation = new StockLocation(GuidGenerator.Create())
            {
                ProductId = line.ProductId,
                LocationType = InventoryLocationTypeEnum.Warehouse,
                LocationId = request.SourceSiteId,
                Quantity = remainingSourceQuantity,
                ReservedQuantity = 0
            };

            await _stockLocationRepository.InsertAsync(sourceLocation, autoSave: true);
            return;
        }

        sourceLocation.Quantity = remainingSourceQuantity;
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
        InventoryLocationTypeEnum targetLocationType)
    {
        if (targetLocationType == InventoryLocationTypeEnum.Vehicle)
        {
            return await _stockLocationRepository.FindAsync(x =>
                x.ProductId == line.ProductId &&
                x.LocationType == InventoryLocationTypeEnum.Vehicle &&
                x.LocationId == request.RequestedVehicleId);
        }

        return await _stockLocationRepository.FindAsync(x =>
            x.ProductId == line.ProductId &&
            x.LocationType == InventoryLocationTypeEnum.Warehouse &&
            x.LocationId == request.TargetSiteId);
    }

    private StockLocation CreateTargetStockLocation(
        MovementRequest request,
        MovementRequestLine line,
        InventoryLocationTypeEnum targetLocationType)
    {
        // Hedef lokasyon arac secimine gore arac veya depo olarak olusturulur.
        return new StockLocation(GuidGenerator.Create())
        {
            ProductId = line.ProductId,
            LocationType = targetLocationType,
            LocationId = targetLocationType == InventoryLocationTypeEnum.Warehouse
                ? request.TargetSiteId
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
            SourceLocationType = InventoryLocationTypeEnum.Warehouse,
            SourceLocationId = request.SourceSiteId,
            TargetLocationType = targetLocationType,
            TargetLocationId = targetLocationType == InventoryLocationTypeEnum.Warehouse
                ? request.TargetSiteId
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

    private static InventoryLocationTypeEnum ResolveTargetLocationType(MovementRequest request)
    {
        // Arac secildiyse malzeme gorevdeki araca, aksi halde hedef depoya gider.
        return request.RequestedVehicleId.HasValue
            ? InventoryLocationTypeEnum.Vehicle
            : InventoryLocationTypeEnum.Warehouse;
    }

    private static InventoryTransactionTypeEnum ResolveTransactionType(InventoryLocationTypeEnum targetLocationType)
    {
        return targetLocationType == InventoryLocationTypeEnum.Vehicle
            ? InventoryTransactionTypeEnum.WarehouseToVehicle
            : InventoryTransactionTypeEnum.WarehouseToWarehouse;
    }
}
