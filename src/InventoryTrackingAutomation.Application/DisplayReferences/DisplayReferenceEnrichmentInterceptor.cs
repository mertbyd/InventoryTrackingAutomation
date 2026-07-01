using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Common;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;
using static InventoryTrackingAutomation.Constants.DisplayReferences.DisplayReferenceConventionConstants;

namespace InventoryTrackingAutomation.Application.DisplayReferences;

/// <summary>
/// AppService donuslerini yakalayip IHasDisplayReferences DTO'larini display degerleriyle otomatik doldurur.
/// </summary>
// Kodsuz enrichment: sarilan her AppService metodu, donusunde IHasDisplayReferences tasiyorsa otomatik
// zenginlestirilir; yeni AppService/DTO eklendiginde ek kod gerekmez. InventoryCacheInterceptor ile ayni
// IAbpInterceptor desenini kullanir.
public sealed class DisplayReferenceEnrichmentInterceptor : IAbpInterceptor, ITransientDependency
{
    private readonly DisplayReferenceEnricher _enricher;

    public DisplayReferenceEnrichmentInterceptor(DisplayReferenceEnricher enricher)
    {
        _enricher = enricher;
    }

    public async Task InterceptAsync(IAbpMethodInvocation invocation)
    {
        // Once asil AppService metodu calisir; manager/mapping davranisi aynen korunur.
        await invocation.ProceedAsync();
        var targets = ExtractTargets(invocation.ReturnValue);
        if (targets.Count > 0)
        {
            await _enricher.EnrichAsync(targets);
        }
    }

    /// <summary>
    /// Donus degerinden zenginlestirilecek DTO'lari cikarir: tekil DTO, DTO koleksiyonu veya
    /// ListResultDto/PagedResultDto gibi Items sarmalayicilari desteklenir.
    /// </summary>
    private static IReadOnlyCollection<IHasDisplayReferences> ExtractTargets(object? returnValue)
    {
        return returnValue switch
        {
            null => Array.Empty<IHasDisplayReferences>(),
            IHasDisplayReferences single => new[] { single },
            IEnumerable enumerable => ExtractFromEnumerable(enumerable),
            _ => ExtractFromItemsWrapper(returnValue)
        };
    }

    /// <summary>
    /// ListResultDto/PagedResultDto gibi sarmalayicilardaki Items koleksiyonundan hedef DTO'lari cikarir.
    /// </summary>
    private static IReadOnlyCollection<IHasDisplayReferences> ExtractFromItemsWrapper(object returnValue)
    {
        return returnValue.GetType().GetProperty(ItemsProperty)?.GetValue(returnValue) is IEnumerable items
            ? ExtractFromEnumerable(items)
            : Array.Empty<IHasDisplayReferences>();
    }

    /// <summary>
    /// Koleksiyon icindeki display reference tasiyan DTO'lari tek noktadan filtreler.
    /// </summary>
    private static IReadOnlyCollection<IHasDisplayReferences> ExtractFromEnumerable(IEnumerable items) =>
        items.OfType<IHasDisplayReferences>().ToList();
}
