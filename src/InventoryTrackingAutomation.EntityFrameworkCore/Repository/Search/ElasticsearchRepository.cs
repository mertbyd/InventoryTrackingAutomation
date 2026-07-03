using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Esql.Extensions;
using InventoryTrackingAutomation.ExceptionCodes;
using InventoryTrackingAutomation.Interface.Search;
using InventoryTrackingAutomation.Search;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Repository.Search;

/// <summary>
/// Elasticsearch index'leri icin generic repository base'i; ES client'a dokunan tek yerdir.
/// </summary>
// islevi: Lambda okumalarini resmi LINQ-to-ES|QL provider'ina cevirtir, yazma/silme operasyonlarini calistirir,
// isimli sorgular icin Query DSL kurulum yardimcilarini saglar.
// sistemdeki gorevi: Yeni bir arama index'i eklendiginde bu base kalitilir; ceviri/DSL/index adi yonetimi tekrar yazilmaz.
public abstract class ElasticsearchRepository<TDocument> : IElasticsearchRepository<TDocument>
    where TDocument : class, ISearchDocument
{
    protected ElasticsearchClient Client { get; }
    protected ElasticsearchOptions Options { get; }

    protected ElasticsearchRepository(ElasticsearchClient client, IOptions<ElasticsearchOptions> options)
    {
        Client = client;
        Options = options.Value;
    }

    /// <summary>
    /// Predicate'e uyan ilk dokumani doner; eslesme yoksa null.
    /// </summary>
    public async Task<TDocument?> GetByAsync(Expression<Func<TDocument, bool>> predicate)
    {
        return await Client.Esql.CreateQuery<TDocument>()
            .From(TDocument.IndexName)
            .Where(predicate)
            .AsEsqlQueryable()
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Predicate'e uyan dokumanlari doner; ceviriyi Elastic'in LINQ-to-ES|QL provider'i yapar.
    /// </summary>
    public async Task<List<TDocument>> GetListByAsync(Expression<Func<TDocument, bool>> predicate, int maxResultCount)
    {
        return await Client.Esql.CreateQuery<TDocument>()
            .From(TDocument.IndexName)
            .Where(predicate)
            .Take(maxResultCount)
            .AsEsqlQueryable()
            .ToListAsync();
    }

    /// <summary>
    /// Dokumani upsert eder; ES erisilemezse false doner, cagiran yazma akisi bozulmaz.
    /// </summary>
    public async Task<bool> IndexAsync(TDocument document)
    {
        var response = await Client.IndexAsync(document, i => i
            .Index(TDocument.IndexName)
            .Id(document.Id.ToString()));
        return response.IsValidResponse;
    }

    /// <summary>
    /// Dokumanlari tek bulk istegiyle upsert eder.
    /// </summary>
    public async Task<bool> IndexManyAsync(IReadOnlyCollection<TDocument> documents)
    {
        var response = await Client.BulkAsync(b => b
            .Index(TDocument.IndexName)
            .IndexMany(documents));
        return response.IsValidResponse && !response.Errors;
    }

    /// <summary>
    /// Dokumani kaynak entity Id'si ile index'ten siler.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await Client.DeleteAsync(TDocument.IndexName, id.ToString());
        return response.IsValidResponse;
    }

    // --- Isimli sorgu yardimcilari: fuzzy/sayfali arama ES|QL'de olmadigindan Query DSL ile burada kurulur. ---

    /// <summary>
    /// Sorguyu sayfali calistirir; toplam kayit sayisi ayni response'tan okunur.
    /// </summary>
    protected async Task<PagedResultDto<TDocument>> SearchPagedAsync(Query query, int skipCount, int maxResultCount)
    {
        var response = await Client.SearchAsync<TDocument>(s => s
            .Indices(TDocument.IndexName)
            .From(skipCount)
            .Size(maxResultCount)
            .Query(query));

        EnsureValid(response);
        return new PagedResultDto<TDocument>(response.Total, response.Documents.ToList());
    }

    /// <summary>
    /// Tum dokumanlari eslestirir.
    /// </summary>
    protected static Query MatchAll() => new MatchAllQuery();

    /// <summary>
    /// Verilen sorgulardan en az biri eslesmelidir (OR).
    /// </summary>
    protected static Query AnyOf(params Query[] queries) => new BoolQuery
    {
        Should = queries,
        MinimumShouldMatch = 1
    };

    /// <summary>
    /// Analiz edilen text alanda fuzzy metin aramasi; tolerans ElasticsearchOptions.Fuzziness'tan gelir.
    /// </summary>
    protected Query FuzzyMatch(Expression<Func<TDocument, object>> field, string text) => new MatchQuery
    {
        Field = Infer.Field(field),
        Query = text,
        Fuzziness = new Fuzziness(Options.Fuzziness)
    };

    /// <summary>
    /// Birebir eslesme (term); text alanlarda analiz edilmemis alt alan .Suffix(SearchFieldSuffixes.Keyword) ile hedeflenir.
    /// </summary>
    protected static Query ExactMatch(Expression<Func<TDocument, object>> field, string value) => new TermQuery
    {
        Field = Infer.Field(field),
        Value = value
    };

    // Okuma tarafinda ES erisilemezse bos sonucla karistirilamaz; arama kullanilamiyor hatasi doner.
    private static void EnsureValid(SearchResponse<TDocument> response)
    {
        if (!response.IsValidResponse)
        {
            throw new BusinessException(GeneralExceptionCodes.SearchUnavailable);
        }
    }
}
