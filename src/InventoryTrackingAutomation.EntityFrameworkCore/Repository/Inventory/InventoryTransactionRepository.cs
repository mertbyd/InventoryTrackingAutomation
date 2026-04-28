using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Interface.Inventory;

namespace InventoryTrackingAutomation.Repository.Stock;

/// <summary>
/// InventoryTransaction entity'si icin EF Core repository implementasyonu.
/// </summary>
public class InventoryTransactionRepository : BaseRepository<InventoryTransaction>, IInventoryTransactionRepository
{
    public InventoryTransactionRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
