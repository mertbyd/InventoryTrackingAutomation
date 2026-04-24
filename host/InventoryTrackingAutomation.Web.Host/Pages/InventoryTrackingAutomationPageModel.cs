using InventoryTrackingAutomation.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace InventoryTrackingAutomation.Pages;

public abstract class InventoryTrackingAutomationPageModel : AbpPageModel
{
    protected InventoryTrackingAutomationPageModel()
    {
        LocalizationResourceType = typeof(InventoryTrackingAutomationResource);
    }
}
