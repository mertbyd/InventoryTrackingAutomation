using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Search;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Interface.Search;

/// <summary>
/// Elasticsearch index'leri icin generic repository arayuzu; ES sorgu DSL'i cagirana sizmasin diye
/// okuma tarafinda EF repository'lerdeki gibi expression predicate kullanilir.
/// </summary>
// islevi: Dokuman okuma/yazma operasyonlarini lambda tabanli sunar; index adi ISearchDocument.IndexName'den cozulur.
// sistemdeki gorevi: ES client cagrilari yalnizca bu repository implementasyonlarinda yasar; AppService ve event handler DSL gormez.
public interface IElasticsearchRepository<TDocument> where TDocument : class, ISearchDocument
{
    /// <summary>
    /// Predicate'e uyan ilk dokumani doner; eslesme yoksa null. Ornek: GetByAsync(x => x.Code == "PRD-001").
    /// Desteklenen ifadeler: ==, &amp;&amp;, ||, Equals, Contains.
    /// </summary>
    Task<TDocument?> GetByAsync(Expression<Func<TDocument, bool>> predicate);

    /// <summary>
    /// Predicate'e uyan dokumanlari sayfali doner. Ornek: GetListByAsync(x => x.CategoryId == id &amp;&amp; x.Name.Contains("vida"), 0, 20).
    /// String alanda Contains fuzzy metin aramasi, == birebir eslesme olarak calisir; x => true tum dokumanlari doner.
    /// </summary>
    Task<PagedResultDto<TDocument>> GetListByAsync(
        Expression<Func<TDocument, bool>> predicate,
        int skipCount,
        int maxResultCount);

    /// <summary>
    /// Dokumani upsert eder (ayni Id varsa uzerine yazar); ES erisilemezse false doner.
    /// </summary>
    Task<bool> IndexAsync(TDocument document);

    /// <summary>
    /// Dokumanlari tek bulk istegiyle upsert eder; reindex ve toplu degisiklikler icin kullanilir.
    /// </summary>
    Task<bool> IndexManyAsync(IReadOnlyCollection<TDocument> documents);

    /// <summary>
    /// Dokumani kaynak entity Id'si ile index'ten siler.
    /// </summary>
    Task<bool> DeleteAsync(Guid id);
}
