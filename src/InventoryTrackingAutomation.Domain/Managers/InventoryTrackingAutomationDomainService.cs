using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;

namespace InventoryTrackingAutomation.Managers;

/// <summary>
/// Domain servislerinde lazy dependency cozumleme altyapisini merkezi olarak tutar.
/// </summary>
public abstract class InventoryTrackingAutomationDomainService : DomainService
{
    private readonly IAbpLazyServiceProvider? _abpLazyServiceProvider;

    protected InventoryTrackingAutomationDomainService()
    {
    }

    protected InventoryTrackingAutomationDomainService(IAbpLazyServiceProvider abpLazyServiceProvider)
    {
        _abpLazyServiceProvider = abpLazyServiceProvider;
    }

    protected TService LazyGetRequiredService<TService>()
        where TService : notnull
    {
        return (_abpLazyServiceProvider ?? LazyServiceProvider).LazyGetRequiredService<TService>();
    }
}
