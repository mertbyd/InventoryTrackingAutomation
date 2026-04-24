using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Stock;
using InventoryTrackingAutomation.Interface.Stock;

namespace InventoryTrackingAutomation.Repository.Stock;

/// <summary>
/// StockMovement entity'si için EF Core repository implementasyonu.
/// </summary>
public class StockMovementRepository : BaseRepository<StockMovement>, IStockMovementRepository
{
    public StockMovementRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
