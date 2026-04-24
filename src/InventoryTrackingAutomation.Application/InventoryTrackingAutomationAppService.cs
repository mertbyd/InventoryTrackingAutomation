using InventoryTrackingAutomation.Localization;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation;

public abstract class InventoryTrackingAutomationAppService : ApplicationService
{
    protected InventoryTrackingAutomationAppService()
    {
        LocalizationResource = typeof(InventoryTrackingAutomationResource);
        ObjectMapperContext = typeof(InventoryTrackingAutomationApplicationModule);
    }
}
