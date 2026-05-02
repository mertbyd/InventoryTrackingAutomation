using InventoryTrackingAutomation.Workflows;
using InventoryTrackingAutomation.EntityFrameworkCore;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Workflows;

/*
 * KURAL: Domain.Tests katmanındaki abstract sınıf, burada (EF Core'da) miras alınır.
 */
public class WorkflowManager_Tests : InventoryTrackingAutomation.Workflows.WorkflowManager_Tests<InventoryTrackingAutomationEntityFrameworkCoreTestModule>
{
    // Test içeriği boş bırakılır. Bütün senaryolar üst sınıftan çalıştırılır.
}
