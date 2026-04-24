using InventoryTrackingAutomation.Localization;
using Volo.Abp.AspNetCore.Components;

namespace InventoryTrackingAutomation.Blazor.Server.Host;

public abstract class InventoryTrackingAutomationComponentBase : AbpComponentBase
{
    protected InventoryTrackingAutomationComponentBase()
    {
        LocalizationResource = typeof(InventoryTrackingAutomationResource);
    }
}
