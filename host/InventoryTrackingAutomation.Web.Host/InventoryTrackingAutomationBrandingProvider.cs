using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation;

[Dependency(ReplaceServices = true)]
public class InventoryTrackingAutomationBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "InventoryTrackingAutomation";
}
