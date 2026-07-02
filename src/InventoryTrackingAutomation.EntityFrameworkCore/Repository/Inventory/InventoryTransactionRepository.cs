using InventoryTrackingAutomation.Models.Movements;
using InventoryTrackingAutomation.Enums.Inventory;
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
/// InventoryTransaction entity'si icin EF Core repository implementasyonu.
/// </summary>
public class InventoryTransactionRepository : BaseRepository<InventoryTransaction>, IInventoryTransactionRepository
{
    public InventoryTransactionRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<TaskVehicleReturnLineModel>> GetVehicleReturnLinesAsync(HashSet<Guid> movementIds, Guid vehicleId)
    {
        var dbContext = await GetDbContextAsync();
        var transactions = await dbContext.InventoryTransactions
            .Where(x =>
                x.RelatedMovementRequestId.HasValue &&
                movementIds.Contains(x.RelatedMovementRequestId.Value) &&
                (
                    (x.TransactionType == InventoryTransactionTypeEnum.WarehouseToVehicle &&
                     x.TargetLocationType == StockLocationTypeEnum.Vehicle &&
                     x.TargetLocationId == vehicleId) ||
                    (x.TransactionType == InventoryTransactionTypeEnum.VehicleToWarehouse &&
                     x.SourceLocationType == StockLocationTypeEnum.Vehicle &&
                     x.SourceLocationId == vehicleId) ||
                    (x.TransactionType == InventoryTransactionTypeEnum.Adjustment &&
                     x.SourceLocationType == StockLocationTypeEnum.Vehicle &&
                     x.SourceLocationId == vehicleId)
                ))
            .ToListAsync();

        return transactions
            .GroupBy(x => x.ProductId)
            .Select(group => new TaskVehicleReturnLineModel(
                group.Key,
                group.Sum(x => x.TransactionType == InventoryTransactionTypeEnum.WarehouseToVehicle
                    ? x.Quantity
                    : -x.Quantity)))
            .Where(x => x.Quantity > 0)
            .ToList();
    }

    public async Task<Guid?> GetLastSourceWarehouseIdAsync(HashSet<Guid> movementIds, Guid vehicleId)
    {
        var dbContext = await GetDbContextAsync();
        var lastTransaction = await dbContext.InventoryTransactions
            .Where(x =>
                x.RelatedMovementRequestId.HasValue &&
                movementIds.Contains(x.RelatedMovementRequestId.Value) &&
                x.TargetLocationType == StockLocationTypeEnum.Vehicle &&
                x.TargetLocationId == vehicleId &&
                x.TransactionType == InventoryTransactionTypeEnum.WarehouseToVehicle)
            .OrderByDescending(x => x.OccurredAt)
            .FirstOrDefaultAsync();

        if (lastTransaction?.SourceLocationType == StockLocationTypeEnum.Warehouse &&
            lastTransaction.SourceLocationId.HasValue)
        {
            return lastTransaction.SourceLocationId.Value;
        }

        return null;
    }

    public override async Task<IQueryable<InventoryTransaction>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).Include(x => x.Product).Include(x => x.RelatedMovementRequest);
    }
}


