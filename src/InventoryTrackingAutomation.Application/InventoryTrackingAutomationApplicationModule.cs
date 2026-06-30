using Microsoft.Extensions.DependencyInjection;
using InventoryTrackingAutomation.Application.Services.Auth;
using InventoryTrackingAutomation.Services.Auth;
using InventoryTrackingAutomation.Application.Services.Movements;
using InventoryTrackingAutomation.Services.Movements;

using Volo.Abp.Modularity;
using Volo.Abp.Application;

namespace InventoryTrackingAutomation;

[DependsOn(
    typeof(InventoryTrackingAutomationDomainModule),
    typeof(InventoryTrackingAutomationApplicationContractsModule),
    typeof(AbpDddApplicationModule)
    )]
public class InventoryTrackingAutomationApplicationModule : AbpModule
{

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        
    }
}
