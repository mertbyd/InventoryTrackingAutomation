using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace InventoryTrackingAutomation;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(InventoryTrackingAutomationHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class InventoryTrackingAutomationConsoleApiClientModule : AbpModule
{

}
