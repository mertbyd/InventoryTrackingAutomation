using Volo.Abp.Modularity;

namespace InventoryTrackingAutomation;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class InventoryTrackingAutomationApplicationTestBase<TStartupModule> : InventoryTrackingAutomationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}

public abstract class InventoryTrackingAutomationApplicationTestBase : InventoryTrackingAutomationApplicationTestBase<InventoryTrackingAutomationApplicationTestModule>
{

}
