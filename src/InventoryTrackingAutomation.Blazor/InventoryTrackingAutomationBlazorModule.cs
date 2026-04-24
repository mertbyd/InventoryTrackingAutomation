using Microsoft.Extensions.DependencyInjection;
using InventoryTrackingAutomation.Blazor.Menus;
using Volo.Abp.AspNetCore.Components.Web.Theming;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;

namespace InventoryTrackingAutomation.Blazor;

[DependsOn(
    typeof(InventoryTrackingAutomationApplicationContractsModule),
    typeof(AbpAspNetCoreComponentsWebThemingModule),
    typeof(AbpAutoMapperModule)
    )]
public class InventoryTrackingAutomationBlazorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<InventoryTrackingAutomationBlazorModule>();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<InventoryTrackingAutomationBlazorAutoMapperProfile>(validate: true);
        });

        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new InventoryTrackingAutomationMenuContributor());
        });

        Configure<AbpRouterOptions>(options =>
        {
            options.AdditionalAssemblies.Add(typeof(InventoryTrackingAutomationBlazorModule).Assembly);
        });
    }
}
