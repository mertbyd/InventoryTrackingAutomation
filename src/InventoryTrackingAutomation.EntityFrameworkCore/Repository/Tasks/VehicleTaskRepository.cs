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
}
