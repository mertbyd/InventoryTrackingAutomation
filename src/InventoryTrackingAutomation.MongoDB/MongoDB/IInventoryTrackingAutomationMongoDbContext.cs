using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace InventoryTrackingAutomation.MongoDB;

[ConnectionStringName(InventoryTrackingAutomationDbProperties.ConnectionStringName)]
public interface IInventoryTrackingAutomationMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
}
