using Volo.Abp.Application;
using Volo.Abp.FluentValidation;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace InventoryTrackingAutomation;

[DependsOn(
    typeof(InventoryTrackingAutomationDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule),
    typeof(AbpFluentValidationModule)
    )]
public class InventoryTrackingAutomationApplicationContractsModule : AbpModule
{

}
