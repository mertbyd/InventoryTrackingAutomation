using Volo.Abp.AspNetCore.Components.Server.Theming;
using Volo.Abp.Modularity;

namespace InventoryTrackingAutomation.Blazor.Server;

[DependsOn(
    typeof(AbpAspNetCoreComponentsServerThemingModule),
    typeof(InventoryTrackingAutomationBlazorModule)
    )]
public class InventoryTrackingAutomationBlazorServerModule : AbpModule
{

}
