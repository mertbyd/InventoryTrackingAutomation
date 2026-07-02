using System.Linq;
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

    /// <summary>
    /// Goreve ait task line kayitlarini getirir.
    /// </summary>
    // islevi: TaskId uzerinden gorev kalemlerini listeler.
    // sistemdeki gorevi: TaskLine artik soft-delete tasimadigi icin sorguda yalnizca is filtresini tutar.
    public async Task<List<TaskLine>> GetByTaskIdAsync(Guid taskId)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.TaskLines
            .Where(x => x.TaskId == taskId)
            .ToListAsync();
    }

    /// <summary>
    /// Gorev ve urun kombinasyonuna ait task line kaydini bulur.
    /// </summary>
    // islevi: Ayni gorev icinde ayni urun kalemi var mi kontrol eder.
    // sistemdeki gorevi: Benzersizlik kontrolu icin sadece gorev ve urun filtresini tasir.
    public async Task<TaskLine?> FindByTaskAndProductAsync(Guid taskId, Guid productId)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.TaskLines
            .FirstOrDefaultAsync(x => x.TaskId == taskId && x.ProductId == productId);
    }

    public override async Task<IQueryable<TaskLine>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).Include(x => x.Task).Include(x => x.Product);
    }
}


