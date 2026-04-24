using InventoryTrackingAutomation.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace InventoryTrackingAutomation;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(InventoryTrackingAutomationEntityFrameworkCoreTestModule)
    )]
public class InventoryTrackingAutomationDomainTestModule : AbpModule
{

}
