using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Search;

namespace InventoryTrackingAutomation.Interface.Search;

/// <summary>
/// Elasticsearch index'leri icin generic repository arayuzu; okuma tarafinda EF repository'lerdeki gibi
/// lambda predicate kullanilir, ceviriyi Elastic'in resmi LINQ-to-ES|QL provider'i yapar.
/// </summary>
// islevi: Lambda tabanli okuma + index yazma/silme sozlesmesini sunar; index adi ISearchDocument.IndexName'den cozulur.
// sistemdeki gorevi: ES client cagrilari yalnizca bu repository implementasyonlarinda yasar; fuzzy/sayfali arama gibi
// ES|QL'in desteklemedigi sorgular index'e ozel repository'de isimli metot olarak tanimlanir.
public interface IElasticsearchRepository<TDocument> where TDocument : class, ISearchDocument
{
    /// <summary>
    /// Predicate'e uyan ilk dokumani doner; eslesme yoksa null. Ornek: GetByAsync(x => x.Code == "PRD-001").
    /// </summary>
    Task<TDocument?> GetByAsync(Expression<Func<TDocument, bool>> predicate);

    /// <summary>
    /// Predicate'e uyan dokumanlari doner. Ornek: GetListByAsync(x => x.CategoryId == categoryId, 20).
    /// Esitlik/karsilastirma, &amp;&amp;, ||, ! ve koleksiyon Contains (IN) desteklenir; fuzzy ve skip sayfalama ES|QL'de yoktur.
    /// </summary>
    Task<List<TDocument>> GetListByAsync(Expression<Func<TDocument, bool>> predicate, int maxResultCount);

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
