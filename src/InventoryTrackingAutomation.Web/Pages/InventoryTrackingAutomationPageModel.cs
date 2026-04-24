using InventoryTrackingAutomation.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace InventoryTrackingAutomation.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class InventoryTrackingAutomationPageModel : AbpPageModel
{
    protected InventoryTrackingAutomationPageModel()
    {
        LocalizationResourceType = typeof(InventoryTrackingAutomationResource);
        ObjectMapperContext = typeof(InventoryTrackingAutomationWebModule);
    }
}
