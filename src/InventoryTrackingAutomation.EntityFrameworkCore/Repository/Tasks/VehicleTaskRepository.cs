using InventoryTrackingAutomation.Models.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Interface.Tasks;

namespace InventoryTrackingAutomation.Repository.Tasks;

/// <summary>
/// VehicleTask entity'si icin EF Core repository implementasyonu.
/// </summary>
public class VehicleTaskRepository : BaseRepository<VehicleTask>, IVehicleTaskRepository
{
    public VehicleTaskRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<TaskVehicleModel>> GetTaskVehiclesByTaskIdAsync(Guid inventoryTaskId)
    {
        var dbContext = await GetDbContextAsync();
        return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
            dbContext.VehicleTasks
                .Where(x => x.TaskId == inventoryTaskId)
                .Select(x => new TaskVehicleModel
                {
                    VehicleTaskId = x.Id,
                    TaskId = x.TaskId,
                    VehicleId = x.VehicleId,
                    AssignedAt = x.AssignedAt,
                    ReleasedAt = x.ReleasedAt
                })
        );
    }

    public override async Task<IQueryable<VehicleTask>> WithDetailsAsync()
    {
        return (await GetQueryableAsync())
            .Include(x => x.Vehicle)
            .Include(x => x.Task)
            .Include(x => x.ResponsibleWorker)
            .Include(x => x.Lines).ThenInclude(l => l.TaskLine).ThenInclude(t => t.Product);
    }
}


