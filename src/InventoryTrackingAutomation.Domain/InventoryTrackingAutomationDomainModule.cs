using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Identity;

namespace InventoryTrackingAutomation;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(InventoryTrackingAutomationDomainSharedModule),
    typeof(AbpPermissionManagementDomainModule),
    typeof(AbpIdentityDomainModule)
)]
public class InventoryTrackingAutomationDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAssemblyOf<InventoryTrackingAutomationDomainModule>();
        context.Services.AddTransient<InventoryTrackingAutomation.Data.InventoryTrackingAutomationDataSeedContributor>();
        context.Services.AddTransient<Volo.Abp.Data.IDataSeedContributor>(sp => sp.GetRequiredService<InventoryTrackingAutomation.Data.InventoryTrackingAutomationDataSeedContributor>());
    }
}
