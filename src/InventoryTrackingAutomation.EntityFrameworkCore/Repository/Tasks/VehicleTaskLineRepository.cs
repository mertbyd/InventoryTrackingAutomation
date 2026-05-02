using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Interface.Tasks;

namespace InventoryTrackingAutomation.Repository.Tasks;

/// <summary>
/// VehicleTaskLine entity'si icin EF Core repository implementasyonu.
/// </summary>
public class VehicleTaskLineRepository : BaseRepository<VehicleTaskLine>, IVehicleTaskLineRepository
{
    public VehicleTaskLineRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<VehicleTaskLine>> GetByVehicleTaskIdAsync(Guid vehicleTaskId)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.VehicleTaskLines
            .Where(x => x.VehicleTaskId == vehicleTaskId && !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<List<VehicleTaskLine>> GetByTaskLineIdsAsync(IReadOnlyCollection<Guid> taskLineIds)
    {
        if (taskLineIds.Count == 0)
        {
            return new List<VehicleTaskLine>();
        }

        var dbContext = await GetDbContextAsync();
        return await dbContext.VehicleTaskLines
            .Where(x => taskLineIds.Contains(x.TaskLineId) && !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<VehicleTaskLine?> FindByVehicleTaskAndTaskLineAsync(Guid vehicleTaskId, Guid taskLineId)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.VehicleTaskLines
            .FirstOrDefaultAsync(x => x.VehicleTaskId == vehicleTaskId && x.TaskLineId == taskLineId && !x.IsDeleted);
    }

    public async Task<int> GetAllocatedQuantityByTaskLineIdAsync(Guid taskLineId, Guid? excludedVehicleTaskLineId = null)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.VehicleTaskLines
            .Where(x =>
                x.TaskLineId == taskLineId &&
                !x.IsDeleted &&
                (!excludedVehicleTaskLineId.HasValue || x.Id != excludedVehicleTaskLineId.Value))
            .SumAsync(x => x.AllocatedQuantity);
    }
}
