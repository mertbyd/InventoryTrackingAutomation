using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

namespace InventoryTrackingAutomation.MongoDB;

[DependsOn(
    typeof(InventoryTrackingAutomationDomainModule),
    typeof(AbpMongoDbModule)
    )]
public class InventoryTrackingAutomationMongoDbModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMongoDbContext<InventoryTrackingAutomationMongoDbContext>(options =>
        {
                /* Add custom repositories here. Example:
                 * options.AddRepository<Question, MongoQuestionRepository>();
                 */
        });
    }
}
