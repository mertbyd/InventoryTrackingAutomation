using InventoryTrackingAutomation.Application.Mappers.Masters;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Search;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.EventHandlers.Search;

/// <summary>
/// Urun degisikliklerinde Elasticsearch urun index'ini guncel tutan event handler.
/// </summary>
public class ProductElasticsearchEventHandler :
    ElasticsearchIndexingEventHandler<Product, ProductIndexDto>,
    ITransientDependency
{
    private static readonly ProductMapper _mapper = new ProductMapper();

    public ProductElasticsearchEventHandler(
        IProductRepository productRepository,
        IProductSearchRepository searchRepository)
        : base(productRepository, searchRepository)
    {
    }

    protected override ProductIndexDto MapToDocument(Product entity)
    {
        return _mapper.MapToIndexDto(entity);
    }
}
