using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace InventoryTrackingAutomation.EntityFrameworkCore;

[ConnectionStringName(InventoryTrackingAutomationDbProperties.ConnectionStringName)]
public interface IInventoryTrackingAutomationDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
