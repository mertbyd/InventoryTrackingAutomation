using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Interface.Tasks;

namespace InventoryTrackingAutomation.Repository.Tasks;

/// <summary>
/// TaskLine entity'si icin EF Core repository implementasyonu.
/// </summary>
public class TaskLineRepository : BaseRepository<TaskLine>, ITaskLineRepository
{
    public TaskLineRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<TaskLine>> GetByTaskIdAsync(Guid taskId)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.TaskLines
            .Where(x => x.TaskId == taskId && !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<TaskLine?> FindByTaskAndProductAsync(Guid taskId, Guid productId)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.TaskLines
            .FirstOrDefaultAsync(x => x.TaskId == taskId && x.ProductId == productId && !x.IsDeleted);
    }
}
