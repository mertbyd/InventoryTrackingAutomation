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
    
}
