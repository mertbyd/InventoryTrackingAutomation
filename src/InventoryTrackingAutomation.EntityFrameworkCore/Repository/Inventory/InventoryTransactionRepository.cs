using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Interface.Inventory;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace InventoryTrackingAutomation.Repository.Stock;

/// <summary>
/// InventoryTransaction entity'si icin EF Core repository implementasyonu.
/// </summary>
public class InventoryTransactionRepository : BaseRepository<InventoryTransaction>, IInventoryTransactionRepository
{
    public InventoryTransactionRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async System.Threading.Tasks.Task<System.Collections.Generic.List<InventoryTrackingAutomation.Models.Movements.TaskVehicleReturnLineModel>> GetVehicleReturnLinesAsync(System.Collections.Generic.HashSet<System.Guid> movementIds, System.Guid vehicleId)
    {
        var dbContext = await GetDbContextAsync();
        var transactions = await dbContext.InventoryTransactions
            .Where(x =>
                x.RelatedMovementRequestId.HasValue &&
                movementIds.Contains(x.RelatedMovementRequestId.Value) &&
                (
                    (x.TransactionType == InventoryTrackingAutomation.Enums.Inventory.InventoryTransactionTypeEnum.WarehouseToVehicle &&
                     x.TargetLocationType == InventoryTrackingAutomation.Enums.Inventory.StockLocationTypeEnum.Vehicle &&
                     x.TargetLocationId == vehicleId) ||
                    (x.TransactionType == InventoryTrackingAutomation.Enums.Inventory.InventoryTransactionTypeEnum.VehicleToWarehouse &&
                     x.SourceLocationType == InventoryTrackingAutomation.Enums.Inventory.StockLocationTypeEnum.Vehicle &&
                     x.SourceLocationId == vehicleId) ||
                    (x.TransactionType == InventoryTrackingAutomation.Enums.Inventory.InventoryTransactionTypeEnum.Adjustment &&
                     x.SourceLocationType == InventoryTrackingAutomation.Enums.Inventory.StockLocationTypeEnum.Vehicle &&
                     x.SourceLocationId == vehicleId)
                ))
            .ToListAsync();

        return transactions
            .GroupBy(x => x.ProductId)
            .Select(group => new InventoryTrackingAutomation.Models.Movements.TaskVehicleReturnLineModel(
                group.Key,
                group.Sum(x => x.TransactionType == InventoryTrackingAutomation.Enums.Inventory.InventoryTransactionTypeEnum.WarehouseToVehicle
                    ? x.Quantity
                    : -x.Quantity)))
            .Where(x => x.Quantity > 0)
            .ToList();
    }

    public async System.Threading.Tasks.Task<System.Guid?> GetLastSourceWarehouseIdAsync(System.Collections.Generic.HashSet<System.Guid> movementIds, System.Guid vehicleId)
    {
        var dbContext = await GetDbContextAsync();
        var lastTransaction = await dbContext.InventoryTransactions
            .Where(x =>
                x.RelatedMovementRequestId.HasValue &&
                movementIds.Contains(x.RelatedMovementRequestId.Value) &&
                x.TargetLocationType == InventoryTrackingAutomation.Enums.Inventory.StockLocationTypeEnum.Vehicle &&
                x.TargetLocationId == vehicleId &&
                x.TransactionType == InventoryTrackingAutomation.Enums.Inventory.InventoryTransactionTypeEnum.WarehouseToVehicle)
            .OrderByDescending(x => x.OccurredAt)
            .FirstOrDefaultAsync();

        if (lastTransaction?.SourceLocationType == InventoryTrackingAutomation.Enums.Inventory.StockLocationTypeEnum.Warehouse &&
            lastTransaction.SourceLocationId.HasValue)
        {
            return lastTransaction.SourceLocationId.Value;
        }

        return null;
    }

    public override async System.Threading.Tasks.Task<System.Linq.IQueryable<InventoryTrackingAutomation.Entities.Inventory.InventoryTransaction>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).Include(x => x.Product).Include(x => x.RelatedMovementRequest);
    }
}

