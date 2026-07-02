using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Interface.Tasks;

namespace InventoryTrackingAutomation.Repository.Tasks;

/// <summary>
/// InventoryTask entity'si icin EF Core repository implementasyonu.
/// </summary>
public class InventoryTaskRepository : BaseRepository<InventoryTask>, IInventoryTaskRepository
{
    public InventoryTaskRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public override async System.Threading.Tasks.Task<System.Linq.IQueryable<InventoryTrackingAutomation.Entities.Tasks.InventoryTask>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).Include(x => x.SourceWarehouse).Include(x => x.TargetWarehouse).Include(x => x.ReturnWarehouse);
    }
}

