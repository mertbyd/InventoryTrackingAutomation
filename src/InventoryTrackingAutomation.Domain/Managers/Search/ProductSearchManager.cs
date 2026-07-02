using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Search;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Search;

/// <summary>
/// Urun arama index'inin domain manager'i; reindex/bakim akislari generic base'ten gelir.
/// </summary>
public class ProductSearchManager : ElasticsearchSearchManager<Product, ProductIndexDto>
{
    protected override IElasticsearchRepository<ProductIndexDto> SearchRepository
        => LazyGetRequiredService<IProductSearchRepository>();

    public ProductSearchManager(IProductRepository repository, IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }
}
