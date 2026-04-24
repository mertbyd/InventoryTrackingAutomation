using Volo.Abp.Modularity;
using Volo.Abp.Localization;
using InventoryTrackingAutomation.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace InventoryTrackingAutomation;

[DependsOn(
    typeof(AbpValidationModule)
)]
public class InventoryTrackingAutomationDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<InventoryTrackingAutomationDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<InventoryTrackingAutomationResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/InventoryTrackingAutomation");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("InventoryTrackingAutomation", typeof(InventoryTrackingAutomationResource));
        });
    }
}
