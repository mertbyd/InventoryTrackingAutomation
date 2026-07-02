using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Masters;

namespace InventoryTrackingAutomation.Repository.Masters;

/// <summary>
/// Worker entity'si için EF Core repository implementasyonu.
/// </summary>
public class WorkerRepository : BaseRepository<Worker>, IWorkerRepository
{
    public WorkerRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public override async Task<IQueryable<Worker>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).Include(x => x.WorkerType).Include(x => x.Department).Include(x => x.DefaultWarehouse).Include(x => x.Manager);
    }
}


