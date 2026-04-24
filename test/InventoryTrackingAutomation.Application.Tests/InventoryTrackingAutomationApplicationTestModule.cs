using Volo.Abp.Modularity;

namespace InventoryTrackingAutomation;

[DependsOn(
    typeof(InventoryTrackingAutomationApplicationModule),
    typeof(InventoryTrackingAutomationDomainTestModule)
    )]
public class InventoryTrackingAutomationApplicationTestModule : AbpModule
{

}
