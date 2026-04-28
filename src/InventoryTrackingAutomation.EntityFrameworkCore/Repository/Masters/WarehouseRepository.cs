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
}
