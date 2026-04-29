using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Localization;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Users;

namespace InventoryTrackingAutomation;

public abstract class InventoryTrackingAutomationAppService : ApplicationService
{
    protected InventoryTrackingAutomationAppService()
    {
        LocalizationResource = typeof(InventoryTrackingAutomationResource);
        ObjectMapperContext = typeof(InventoryTrackingAutomationApplicationModule);
    }

    protected async Task<Guid> ResolveCurrentWorkerIdAsync()
    {
        var workerRepository = LazyServiceProvider.GetRequiredService<IWorkerRepository>();
        var userId = CurrentUser.GetId();
        var worker = await workerRepository.FindAsync(w => w.UserId == userId);
        if (worker == null)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.Workers.NotFound)
                .WithData("UserId", userId);
        }
        return worker.Id;
    }
}
