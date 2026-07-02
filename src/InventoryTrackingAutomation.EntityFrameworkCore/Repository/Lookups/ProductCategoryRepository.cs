using Microsoft.EntityFrameworkCore;
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

    public override async System.Threading.Tasks.Task<System.Linq.IQueryable<InventoryTrackingAutomation.Entities.Lookups.ProductCategory>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).Include(x => x.Parent);
    }
}

