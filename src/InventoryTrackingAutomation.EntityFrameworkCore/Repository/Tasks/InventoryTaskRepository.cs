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
}
