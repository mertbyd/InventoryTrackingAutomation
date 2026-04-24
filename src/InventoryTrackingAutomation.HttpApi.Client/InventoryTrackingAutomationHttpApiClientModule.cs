using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace InventoryTrackingAutomation;

[DependsOn(
    typeof(InventoryTrackingAutomationApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class InventoryTrackingAutomationHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(InventoryTrackingAutomationApplicationContractsModule).Assembly,
            InventoryTrackingAutomationRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<InventoryTrackingAutomationHttpApiClientModule>();
        });

    }
}
