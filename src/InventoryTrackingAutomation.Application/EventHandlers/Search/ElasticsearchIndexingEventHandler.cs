using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Interface;
using InventoryTrackingAutomation.Interface.Search;
using InventoryTrackingAutomation.Search;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace InventoryTrackingAutomation.Application.EventHandlers.Search;

/// <summary>
/// Arama index'i olan entity'lerin generic event handler base'i; create/update/delete'te index'i otomatik gunceller.
/// </summary>
// islevi: ABP local entity event'lerini dinler; create/update'te dokumani upsert, delete'te index'ten siler.
// sistemdeki gorevi: Yeni bir entity index'e girdiginde bu base kalitilip sadece mapper cagrisi verilir; handler mantigi tekrar yazilmaz.
public abstract class ElasticsearchIndexingEventHandler<TEntity, TDocument> :
    ILocalEventHandler<EntityCreatedEventData<TEntity>>,
    ILocalEventHandler<EntityUpdatedEventData<TEntity>>,
    ILocalEventHandler<EntityDeletedEventData<TEntity>>
    where TEntity : class, IEntity<Guid>
    where TDocument : class, ISearchDocument
{
    private readonly IBaseRepository<TEntity> _repository;
    private readonly IElasticsearchRepository<TDocument> _searchRepository;

    protected ElasticsearchIndexingEventHandler(
        IBaseRepository<TEntity> repository,
        IElasticsearchRepository<TDocument> searchRepository)
    {
        _repository = repository;
        _searchRepository = searchRepository;
    }

    // Entity -> index dokumani donusumunun tek entity'ye ozel noktasi; Mapperly mapper cagrisi tureyen handler'da yasar.
    protected abstract TDocument MapToDocument(TEntity entity);

    public async Task HandleEventAsync(EntityCreatedEventData<TEntity> eventData)
    {
        await IndexAsync(eventData.Entity.Id);
    }

    public async Task HandleEventAsync(EntityUpdatedEventData<TEntity> eventData)
    {
        await IndexAsync(eventData.Entity.Id);
    }

    public async Task HandleEventAsync(EntityDeletedEventData<TEntity> eventData)
    {
        await _searchRepository.DeleteAsync(eventData.Entity.Id);
    }

    // Denormalize display alanlari dolu gelsin diye entity navigation detaylariyla yeniden yuklenir.
    private async Task IndexAsync(Guid entityId)
    {
        var entity = await _repository.FindAsync(entityId, includeDetails: true);
        if (entity == null)
        {
            return;
        }

        await _searchRepository.IndexAsync(MapToDocument(entity));
    }
}
