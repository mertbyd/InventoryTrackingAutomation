using System.Threading.Tasks;
using InventoryTrackingAutomation.Events.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace InventoryTrackingAutomation.EventHandlers.Cache;

/// <summary>
/// CacheInvalidationEto aldığında event içindeki tüm cache key'lerini Redis'ten temizler.
/// </summary>
public class CacheInvalidationHandler : ILocalEventHandler<CacheInvalidationEto>, ITransientDependency
{
    private readonly IDistributedCache _cache;

    public CacheInvalidationHandler(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task HandleEventAsync(CacheInvalidationEto eventData)
    {
        foreach (var key in eventData.Keys)
            await _cache.RemoveAsync(key);
    }
}
