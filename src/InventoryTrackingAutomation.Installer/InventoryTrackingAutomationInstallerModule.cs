using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace InventoryTrackingAutomation;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class InventoryTrackingAutomationInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<InventoryTrackingAutomationInstallerModule>();
        });
    }
}
