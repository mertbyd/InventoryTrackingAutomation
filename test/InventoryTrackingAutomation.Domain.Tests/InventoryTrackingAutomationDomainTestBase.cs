using Volo.Abp.Modularity;

namespace InventoryTrackingAutomation;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class InventoryTrackingAutomationDomainTestBase<TStartupModule> : InventoryTrackingAutomationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}

public abstract class InventoryTrackingAutomationDomainTestBase : InventoryTrackingAutomationDomainTestBase<InventoryTrackingAutomationDomainTestModule>
{

}
