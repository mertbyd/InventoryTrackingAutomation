using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Masters;

namespace InventoryTrackingAutomation.Repository.Masters;

/// <summary>
/// Vehicle entity'si için EF Core repository implementasyonu.
/// </summary>
public class VehicleRepository : BaseRepository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public override async System.Threading.Tasks.Task<System.Linq.IQueryable<InventoryTrackingAutomation.Entities.Masters.Vehicle>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).Include(x => x.VehicleType);
    }
}

