using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;

namespace InventoryTrackingAutomation.Repository.Lookups;

/// <summary>
/// VehicleType entity'si için EF Core repository implementasyonu.
/// </summary>
public class VehicleTypeRepository : BaseRepository<VehicleType>, IVehicleTypeRepository
{
    public VehicleTypeRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
