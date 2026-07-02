using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Interface;
using InventoryTrackingAutomation.Interface.Search;
using InventoryTrackingAutomation.Search;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;

namespace InventoryTrackingAutomation.Managers.Search;

/// <summary>
/// Arama index'i olan entity'lerin domain manager base'i; index bakim akislarini tek yerde toplar.
/// </summary>
// islevi: Reindex dongusunu (sayfali okuma, bulk index, hata karari) generic olarak isletir.
// sistemdeki gorevi: Yeni bir entity index'e girdiginde bu base kalitilir; reindex/batch mantigi tekrar yazilmaz.
public abstract class ElasticsearchSearchManager<TEntity, TDocument> : BaseManager<TEntity>
    where TEntity : class, IEntity<Guid>
    where TDocument : class, ISearchDocument
{
    private ElasticsearchOptions _options => LazyGetRequiredService<IOptions<ElasticsearchOptions>>().Value;

    // Tureyen manager kendi index'inin Elasticsearch repository'sini gosterir.
    protected abstract IElasticsearchRepository<TDocument> SearchRepository { get; }

    protected ElasticsearchSearchManager(
        IBaseRepository<TEntity> repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    /// <summary>
    /// Index'i kaynak tablodaki guncel veriden bastan kurar; index'lenen dokuman sayisini doner.
    /// Entity -> dokuman donusumu cagiran katmanin Mapperly mapper'indan delegate olarak gelir.
    /// </summary>
    public async Task<long> ReindexAsync(Func<List<TEntity>, List<TDocument>> mapToDocuments)
    {
        long totalIndexed = 0;
        var skip = 0;

        while (true)
        {
            // Denormalize display alanlari dokumana girsin diye entity'ler navigation detaylariyla okunur.
            var entities = await Repository.GetPagedListAsync(
                skip, _options.ReindexBatchSize, nameof(IEntity<Guid>.Id), includeDetails: true);
            if (entities.Count == 0)
            {
                break;
            }

            var documents = mapToDocuments(entities);
            if (!await SearchRepository.IndexManyAsync(documents))
            {
                throw new BusinessException(GeneralExceptionCodes.SearchUnavailable);
            }

            totalIndexed += documents.Count;
            skip += entities.Count;

            if (entities.Count < _options.ReindexBatchSize)
            {
                break;
            }
        }

        return totalIndexed;
    }
}
