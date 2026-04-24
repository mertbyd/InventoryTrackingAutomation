using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Stock;
using InventoryTrackingAutomation.Interface.Stock;

namespace InventoryTrackingAutomation.Repository.Stock;

/// <summary>
/// ProductStock entity'si için EF Core repository implementasyonu.
/// </summary>
public class ProductStockRepository : BaseRepository<ProductStock>, IProductStockRepository
{
    public ProductStockRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
