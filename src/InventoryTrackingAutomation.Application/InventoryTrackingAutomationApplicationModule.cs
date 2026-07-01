using Microsoft.Extensions.DependencyInjection;
using InventoryTrackingAutomation.Application.Services.Auth;
using InventoryTrackingAutomation.Services.Auth;
using InventoryTrackingAutomation.Application.Services.Movements;
using InventoryTrackingAutomation.Services.Movements;

using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Application;
using Volo.Abp.Application.Services;
using Volo.Abp.Collections;
using Volo.Abp.DependencyInjection;
using InventoryTrackingAutomation.Application.DisplayReferences;

namespace InventoryTrackingAutomation;

[DependsOn(
    typeof(InventoryTrackingAutomationDomainModule),
    typeof(InventoryTrackingAutomationApplicationContractsModule),
    typeof(AbpDddApplicationModule)
    )]
public class InventoryTrackingAutomationApplicationModule : AbpModule
{

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        // Kodsuz display enrichment: tum AppService'ler proxy'lenir; interceptor donuste
        // IHasDisplayReferences tasiyan DTO'lari otomatik doldurur. Yeni AppService/DTO ek kayit gerektirmez.
        context.Services.OnRegistered(registration =>
        {
            if (typeof(IApplicationService).IsAssignableFrom(registration.ImplementationType))
            {
                registration.Interceptors.TryAdd<DisplayReferenceEnrichmentInterceptor>();
            }
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {

    }
}
