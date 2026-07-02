using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Masters;

namespace InventoryTrackingAutomation.Repository.Masters;

/// <summary>
/// Warehouse entity'si için EF Core repository implementasyonu.
/// </summary>
public class WarehouseRepository : BaseRepository<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public override async Task<IQueryable<Warehouse>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).Include(x => x.ManagerWorker);
    }
}


