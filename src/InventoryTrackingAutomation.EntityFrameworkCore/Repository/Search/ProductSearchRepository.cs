using Elastic.Clients.Elasticsearch;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Interface.Search;
using InventoryTrackingAutomation.Search;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Repository.Search;

/// <summary>
/// Urun arama index'i (inventory-products) icin Elasticsearch repository implementasyonu.
/// </summary>
public class ProductSearchRepository : ElasticsearchRepository<ProductIndexDto>, IProductSearchRepository, ITransientDependency
{
    public ProductSearchRepository(ElasticsearchClient client, IOptions<ElasticsearchOptions> options)
        : base(client, options)
    {
    }
}
