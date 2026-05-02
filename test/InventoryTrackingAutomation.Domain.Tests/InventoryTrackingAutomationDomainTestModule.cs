using Volo.Abp.Modularity;

namespace InventoryTrackingAutomation;

[DependsOn(
    typeof(InventoryTrackingAutomationTestBaseModule),
    typeof(InventoryTrackingAutomationDomainModule)
    )]
public class InventoryTrackingAutomationDomainTestModule : AbpModule
{

}
