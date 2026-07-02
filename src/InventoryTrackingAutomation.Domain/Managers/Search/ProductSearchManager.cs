using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Search;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Search;

/// <summary>
/// Ürün arama domain manager'ı — Elasticsearch urun index'inin tek sahibidir.
/// </summary>
//işlevi: Urun dokumanlarini index'e yazar/siler, metin aramasini calistirir ve index'i bastan kurar.
//sistemdeki görevi: Elasticsearch'e dokunan tum urun arama isleri burada toplanir; AppService ve event handler ince kalir.
public class ProductSearchManager : BaseManager<Product>
{
    private ElasticsearchClient _elasticClient => LazyGetRequiredService<ElasticsearchClient>();
    private IProductCategoryRepository _categoryRepository => LazyGetRequiredService<IProductCategoryRepository>();
    private IUnitTypeRepository _unitTypeRepository => LazyGetRequiredService<IUnitTypeRepository>();
    private ElasticsearchOptions _options => LazyGetRequiredService<IOptions<ElasticsearchOptions>>().Value;

    public ProductSearchManager(IProductRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    /// <summary>
    /// Keyword'u urun adinda fuzzy, urun kodunda birebir arar; bos keyword tum kayitlari sayfali doner.
    /// </summary>
    //işlevi: Etki alanı kuralını veya validasyonunu işletir.
    //sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<PagedResultDto<ProductIndexDto>> SearchAsync(string? keyword, int skipCount, int maxResultCount)
    {
        var response = await _elasticClient.SearchAsync<ProductIndexDto>(s => s
            .Indices(SearchIndexNames.Products)
            .From(skipCount)
            .Size(maxResultCount)
            .Query(q =>
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    q.MatchAll(m => { });
                    return;
                }

                q.Bool(b => b.Should(
                    sh => sh.Match(m => m
                        .Field(f => f.Name)
                        .Query(keyword)
                        .Fuzziness(new Fuzziness(_options.Fuzziness))),
                    sh => sh.Match(m => m
                        .Field(f => f.Code)
                        .Query(keyword))));
            }));

        if (!response.IsValidResponse)
        {
            throw new BusinessException(GeneralExceptionCodes.SearchUnavailable);
        }

        return new PagedResultDto<ProductIndexDto>(response.Total, response.Documents.ToList());
    }

    /// <summary>
    /// Urun dokumanini lookup adlariyla zenginlestirip index'e upsert eder.
    /// </summary>
    // Client hata durumunda exception firlatmaz (IsValidResponse doner); ES kapali olsa bile urun yazma akisi bozulmaz.
    public async Task IndexAsync(Product product)
    {
        var categoryName = product.CategoryId.HasValue
            ? (await _categoryRepository.FindAsync(product.CategoryId.Value))?.Name
            : null;
        var unitTypeName = (await _unitTypeRepository.FindAsync(product.UnitTypeId))?.Name;

        var document = MapToDocument(product, categoryName, unitTypeName);

        await _elasticClient.IndexAsync(document, i => i
            .Index(SearchIndexNames.Products)
            .Id(document.Id.ToString()));
    }

    /// <summary>
    /// Urun dokumanini index'ten siler.
    /// </summary>
    public async Task RemoveFromIndexAsync(Guid productId)
    {
        await _elasticClient.DeleteAsync(SearchIndexNames.Products, productId.ToString());
    }

    /// <summary>
    /// Urun index'ini PostgreSQL'deki guncel veriden bastan kurar; index'lenen dokuman sayisini doner.
    /// </summary>
    public async Task<long> ReindexAsync()
    {
        long totalIndexed = 0;
        var skip = 0;

        while (true)
        {
            var products = await Repository.GetPagedListAsync(skip, _options.ReindexBatchSize, nameof(Product.Id));
            if (products.Count == 0)
            {
                break;
            }

            var documents = await MapToDocumentsAsync(products);
            var response = await _elasticClient.BulkAsync(b => b
                .Index(SearchIndexNames.Products)
                .IndexMany(documents));

            if (!response.IsValidResponse || response.Errors)
            {
                throw new BusinessException(GeneralExceptionCodes.SearchUnavailable);
            }

            totalIndexed += documents.Count;
            skip += products.Count;

            if (products.Count < _options.ReindexBatchSize)
            {
                break;
            }
        }

        return totalIndexed;
    }

    // Lookup adlari tek sorgu + dictionary ile cozulur; dongude DB cagrisi yapilmaz.
    private async Task<List<ProductIndexDto>> MapToDocumentsAsync(List<Product> products)
    {
        var categoryIds = products
            .Where(p => p.CategoryId.HasValue)
            .Select(p => p.CategoryId!.Value)
            .Distinct()
            .ToList();
        var unitTypeIds = products
            .Select(p => p.UnitTypeId)
            .Distinct()
            .ToList();

        var categoryNames = (await _categoryRepository.GetListAsync(c => categoryIds.Contains(c.Id)))
            .ToDictionary(c => c.Id, c => c.Name);
        var unitTypeNames = (await _unitTypeRepository.GetListAsync(u => unitTypeIds.Contains(u.Id)))
            .ToDictionary(u => u.Id, u => u.Name);

        return products
            .Select(p => MapToDocument(
                p,
                p.CategoryId.HasValue ? categoryNames.GetValueOrDefault(p.CategoryId.Value) : null,
                unitTypeNames.GetValueOrDefault(p.UnitTypeId)))
            .ToList();
    }

    private static ProductIndexDto MapToDocument(Product product, string? categoryName, string? unitTypeName)
    {
        return new ProductIndexDto
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            CategoryId = product.CategoryId,
            CategoryName = categoryName,
            UnitTypeId = product.UnitTypeId,
            UnitTypeName = unitTypeName
        };
    }
}
