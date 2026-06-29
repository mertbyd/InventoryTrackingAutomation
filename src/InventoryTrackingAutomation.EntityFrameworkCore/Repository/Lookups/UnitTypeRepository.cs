using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;

namespace InventoryTrackingAutomation.Repository.Lookups;

/// <summary>
/// UnitType entity'si için EF Core repository implementasyonu.
/// </summary>
public class UnitTypeRepository : BaseRepository<UnitType>, IUnitTypeRepository
{
    public UnitTypeRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
