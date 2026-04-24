using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace InventoryTrackingAutomation.MongoDB;

[ConnectionStringName(InventoryTrackingAutomationDbProperties.ConnectionStringName)]
public class InventoryTrackingAutomationMongoDbContext : AbpMongoDbContext, IInventoryTrackingAutomationMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigureInventoryTrackingAutomation();
    }
}
