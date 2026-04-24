using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace InventoryTrackingAutomation;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(InventoryTrackingAutomationDomainSharedModule)
)]
public class InventoryTrackingAutomationDomainModule : AbpModule
{

}
