using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;

namespace InventoryTrackingAutomation.Repository.Lookups;

/// <summary>
/// ProductCategory entity'si için EF Core repository implementasyonu.
/// </summary>
public class ProductCategoryRepository : BaseRepository<ProductCategory>, IProductCategoryRepository
{
    public ProductCategoryRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
