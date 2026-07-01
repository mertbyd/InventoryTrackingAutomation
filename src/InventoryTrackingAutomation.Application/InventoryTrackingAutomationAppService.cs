using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Localization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

namespace InventoryTrackingAutomation;

public abstract class InventoryTrackingAutomationAppService : ApplicationService
{
    private readonly IAbpLazyServiceProvider? _abpLazyServiceProvider;

    protected InventoryTrackingAutomationAppService()
    {
        LocalizationResource = typeof(InventoryTrackingAutomationResource);
        ObjectMapperContext = typeof(InventoryTrackingAutomationApplicationModule);
    }

    protected InventoryTrackingAutomationAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : this()
    {
        _abpLazyServiceProvider = abpLazyServiceProvider;
    }

    protected TService LazyGetRequiredService<TService>()
        where TService : notnull
    {
        return (_abpLazyServiceProvider ?? LazyServiceProvider).LazyGetRequiredService<TService>();
    }

    protected async Task<Guid> ResolveCurrentWorkerIdAsync()
    {
        var workerRepository = LazyGetRequiredService<IWorkerRepository>();
        var userId = CurrentUser.GetId();
        var worker = await workerRepository.FindAsync(w => w.UserId == userId);
        if (worker == null)
        {
            throw new BusinessException(WorkerExceptionCodes.NotFound);
        }
        return worker.Id;
    }
}
