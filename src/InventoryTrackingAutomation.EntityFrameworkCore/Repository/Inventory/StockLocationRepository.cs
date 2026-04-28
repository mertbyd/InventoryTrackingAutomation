using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Interface.Inventory;

namespace InventoryTrackingAutomation.Repository.Stock;

/// <summary>
/// StockLocation entity'si icin EF Core repository implementasyonu.
/// </summary>
public class StockLocationRepository : BaseRepository<StockLocation>, IStockLocationRepository
{
    public StockLocationRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
