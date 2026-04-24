using Volo.Abp.AspNetCore.Components.WebAssembly.Theming;
using Volo.Abp.Modularity;

namespace InventoryTrackingAutomation.Blazor.WebAssembly;

[DependsOn(
    typeof(InventoryTrackingAutomationBlazorModule),
    typeof(InventoryTrackingAutomationHttpApiClientModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyThemingModule)
    )]
public class InventoryTrackingAutomationBlazorWebAssemblyModule : AbpModule
{

}
