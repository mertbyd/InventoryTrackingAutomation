using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Interface.Search;
using InventoryTrackingAutomation.Search;
using Microsoft.Extensions.Options;
using Volo.Abp.Application.Dtos;
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

    /// <summary>
    /// Keyword'u urun adinda fuzzy, urun kodunda birebir arar; bos keyword tum kayitlari sayfali doner.
    /// </summary>
    public async Task<PagedResultDto<ProductIndexDto>> SearchAsync(string? keyword, int skipCount, int maxResultCount)
    {
        return await SearchPagedAsync(KeywordQuery(keyword), skipCount, maxResultCount);
    }

    private Query KeywordQuery(string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return MatchAll();
        }

        return AnyOf(
            FuzzyMatch(x => x.Name, keyword),
            ExactMatch(x => x.Code.Suffix(SearchFieldSuffixes.Keyword), keyword));
    }
}
