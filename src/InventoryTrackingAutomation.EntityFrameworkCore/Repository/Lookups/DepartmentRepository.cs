using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;

namespace InventoryTrackingAutomation.Repository.Lookups;

/// <summary>
/// Department entity'si için EF Core repository implementasyonu.
/// </summary>
public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
