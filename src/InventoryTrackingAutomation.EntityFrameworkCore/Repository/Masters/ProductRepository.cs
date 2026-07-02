using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Masters;

namespace InventoryTrackingAutomation.Repository.Masters;

/// <summary>
/// Product entity'si için EF Core repository implementasyonu.
/// </summary>
public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public override async Task<IQueryable<Product>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).Include(x => x.Category).Include(x => x.UnitType);
    }
}


