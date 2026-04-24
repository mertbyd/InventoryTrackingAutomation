using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace InventoryTrackingAutomation.Blazor.Server.Host;

[Dependency(ReplaceServices = true)]
public class InventoryTrackingAutomationBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "InventoryTrackingAutomation";
}
