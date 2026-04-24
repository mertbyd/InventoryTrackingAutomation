using Volo.Abp;
using Volo.Abp.MongoDB;

namespace InventoryTrackingAutomation.MongoDB;

public static class InventoryTrackingAutomationMongoDbContextExtensions
{
    public static void ConfigureInventoryTrackingAutomation(
        this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
