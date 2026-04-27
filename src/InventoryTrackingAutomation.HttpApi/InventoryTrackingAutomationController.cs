using InventoryTrackingAutomation.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace InventoryTrackingAutomation;

public abstract class InventoryTrackingAutomationController : AbpControllerBase
{
    protected InventoryTrackingAutomationController()
    {
        LocalizationResource = typeof(InventoryTrackingAutomationResource);
    }
}
