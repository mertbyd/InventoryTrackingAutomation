using InventoryTrackingAutomation.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation;

public abstract class InventoryTrackingAutomationController : AbpControllerBase
{
    private readonly IAbpLazyServiceProvider? _abpLazyServiceProvider;

    protected InventoryTrackingAutomationController()
    {
        LocalizationResource = typeof(InventoryTrackingAutomationResource);
    }

    protected InventoryTrackingAutomationController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : this()
    {
        _abpLazyServiceProvider = abpLazyServiceProvider;
    }

    protected TService LazyGetRequiredService<TService>()
        where TService : notnull
    {
        return (_abpLazyServiceProvider ?? LazyServiceProvider).LazyGetRequiredService<TService>();
    }
}
