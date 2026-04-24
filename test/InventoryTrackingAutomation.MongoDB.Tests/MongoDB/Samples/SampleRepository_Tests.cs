using InventoryTrackingAutomation.Samples;
using Xunit;

namespace InventoryTrackingAutomation.MongoDB.Samples;

[Collection(MongoTestCollection.Name)]
public class SampleRepository_Tests : SampleRepository_Tests<InventoryTrackingAutomationMongoDbTestModule>
{
    /* Don't write custom repository tests here, instead write to
     * the base class.
     * One exception can be some specific tests related to MongoDB.
     */
}
