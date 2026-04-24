using Localization.Resources.AbpUi;
using InventoryTrackingAutomation.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryTrackingAutomation;

[DependsOn(
    typeof(InventoryTrackingAutomationApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class InventoryTrackingAutomationHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(InventoryTrackingAutomationHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<InventoryTrackingAutomationResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
