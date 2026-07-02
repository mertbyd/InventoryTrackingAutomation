using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
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
// islevi: Lambda predicate'i translator ile ES sorgusuna cevirip okuma yapar; index yazma/silme operasyonlarini calistirir.
// sistemdeki gorevi: Yeni bir arama index'i eklendiginde bu base kalitilir; ES DSL ve index adi yonetimi tekrar yazilmaz.
public abstract class ElasticsearchRepository<TDocument> : IElasticsearchRepository<TDocument>
    where TDocument : class, ISearchDocument
{
    private readonly ElasticsearchClient _client;
    private readonly ElasticsearchOptions _options;

    protected ElasticsearchRepository(ElasticsearchClient client, IOptions<ElasticsearchOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    /// <summary>
    /// Predicate'e uyan ilk dokumani doner; eslesme yoksa null.
    /// </summary>
    public async Task<TDocument?> GetByAsync(Expression<Func<TDocument, bool>> predicate)
    {
        var query = ElasticsearchPredicateTranslator.Translate(predicate, _options.Fuzziness);
        var response = await _client.SearchAsync<TDocument>(s => s
            .Indices(TDocument.IndexName)
            .Size(1)
            .Query(query));

        EnsureValid(response);
        return response.Documents.FirstOrDefault();
    }

    /// <summary>
    /// Predicate'e uyan dokumanlari sayfali doner.
    /// </summary>
    public async Task<PagedResultDto<TDocument>> GetListByAsync(
        Expression<Func<TDocument, bool>> predicate,
        int skipCount,
        int maxResultCount)
    {
        var query = ElasticsearchPredicateTranslator.Translate(predicate, _options.Fuzziness);
        var response = await _client.SearchAsync<TDocument>(s => s
            .Indices(TDocument.IndexName)
            .From(skipCount)
            .Size(maxResultCount)
            .Query(query));

        EnsureValid(response);
        return new PagedResultDto<TDocument>(response.Total, response.Documents.ToList());
    }

    /// <summary>
    /// Dokumani upsert eder; ES erisilemezse false doner, cagiran yazma akisi bozulmaz.
    /// </summary>
    public async Task<bool> IndexAsync(TDocument document)
    {
        var response = await _client.IndexAsync(document, i => i
            .Index(TDocument.IndexName)
            .Id(document.Id.ToString()));

        return response.IsValidResponse;
    }

    /// <summary>
    /// Dokumanlari tek bulk istegiyle upsert eder.
    /// </summary>
    public async Task<bool> IndexManyAsync(IReadOnlyCollection<TDocument> documents)
    {
        var response = await _client.BulkAsync(b => b
            .Index(TDocument.IndexName)
            .IndexMany(documents));

        return response.IsValidResponse && !response.Errors;
    }

    /// <summary>
    /// Dokumani kaynak entity Id'si ile index'ten siler.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _client.DeleteAsync(TDocument.IndexName, id.ToString());
        return response.IsValidResponse;
    }

    // Okuma tarafinda ES erisilemezse bos sonucla karistirilamaz; arama kullanilamiyor hatasi doner.
    private static void EnsureValid(SearchResponse<TDocument> response)
    {
        if (!response.IsValidResponse)
        {
            throw new BusinessException(GeneralExceptionCodes.SearchUnavailable);
        }
    }
}
