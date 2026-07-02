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

    public async System.Threading.Tasks.Task<System.Collections.Generic.List<InventoryTrackingAutomation.Models.Tasks.TaskVehicleModel>> GetTaskVehiclesByTaskIdAsync(System.Guid inventoryTaskId)
    {
        var dbContext = await GetDbContextAsync();
        return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
            dbContext.VehicleTasks
                .Where(x => x.TaskId == inventoryTaskId)
                .Select(x => new InventoryTrackingAutomation.Models.Tasks.TaskVehicleModel
                {
                    VehicleTaskId = x.Id,
                    TaskId = x.TaskId,
                    VehicleId = x.VehicleId,
                    AssignedAt = x.AssignedAt,
                    ReleasedAt = x.ReleasedAt
                })
        );
    }

    public override async System.Threading.Tasks.Task<System.Linq.IQueryable<InventoryTrackingAutomation.Entities.Tasks.VehicleTask>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).Include(x => x.Vehicle).Include(x => x.Task).Include(x => x.ResponsibleWorker);
    }
}

