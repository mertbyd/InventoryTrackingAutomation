using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Managers.Search;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace InventoryTrackingAutomation.EventHandlers.Search;

/// <summary>
/// Urun eklendiginde/guncellendiginde/silindiginde Elasticsearch urun index'ini otomatik gunceller.
/// </summary>
// islevi: ABP local entity event'lerini dinler; create/update'te dokumani upsert, delete'te index'ten siler.
// sistemdeki gorevi: Indexleme AppService/manager cagri zincirine sizmaz; urun yazma akisi degismeden arama index'i guncel kalir.
public class ProductElasticsearchEventHandler :
    ILocalEventHandler<EntityCreatedEventData<Product>>,
    ILocalEventHandler<EntityUpdatedEventData<Product>>,
    ILocalEventHandler<EntityDeletedEventData<Product>>,
    ITransientDependency
{
    private readonly ProductSearchManager _productSearchManager;

    public ProductElasticsearchEventHandler(ProductSearchManager productSearchManager)
    {
        _productSearchManager = productSearchManager;
    }

    public async Task HandleEventAsync(EntityCreatedEventData<Product> eventData)
    {
        await _productSearchManager.IndexAsync(eventData.Entity);
    }

    public async Task HandleEventAsync(EntityUpdatedEventData<Product> eventData)
    {
        await _productSearchManager.IndexAsync(eventData.Entity);
    }

    public async Task HandleEventAsync(EntityDeletedEventData<Product> eventData)
    {
        await _productSearchManager.RemoveFromIndexAsync(eventData.Entity.Id);
    }
}
