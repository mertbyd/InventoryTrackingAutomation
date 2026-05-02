using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers;

/// <summary>
/// DomainService turevi olmayan domain yardimci siniflarinda lazy dependency cozumleme altyapisini merkezi olarak tutar.
/// </summary>
public abstract class InventoryTrackingAutomationLazyService
{
    private readonly IAbpLazyServiceProvider? _abpLazyServiceProvider;

    public IAbpLazyServiceProvider LazyServiceProvider { get; set; } = default!;

    protected InventoryTrackingAutomationLazyService()
    {
    }

    protected InventoryTrackingAutomationLazyService(IAbpLazyServiceProvider abpLazyServiceProvider)
    {
        _abpLazyServiceProvider = abpLazyServiceProvider;
    }

    protected TService LazyGetRequiredService<TService>()
        where TService : notnull
    {
        return (_abpLazyServiceProvider ?? LazyServiceProvider).LazyGetRequiredService<TService>();
    }
}
