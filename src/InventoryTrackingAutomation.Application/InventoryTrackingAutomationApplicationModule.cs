using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.Application;

namespace InventoryTrackingAutomation;

[DependsOn(
    typeof(InventoryTrackingAutomationDomainModule),
    typeof(InventoryTrackingAutomationApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
    )]
public class InventoryTrackingAutomationApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<InventoryTrackingAutomationApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<InventoryTrackingAutomationApplicationModule>(validate: true);
        });
    }
}
