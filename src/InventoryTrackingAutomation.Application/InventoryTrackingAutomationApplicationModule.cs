using Microsoft.Extensions.DependencyInjection;
using InventoryTrackingAutomation.Application.Services.Auth;
using InventoryTrackingAutomation.Services.Auth;
using InventoryTrackingAutomation.Application.Services.Movements;
using InventoryTrackingAutomation.Services.Movements;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.Application;

namespace InventoryTrackingAutomation;

[DependsOn(
    typeof(InventoryTrackingAutomationDomainModule),
    typeof(InventoryTrackingAutomationApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
    )]
public class InventoryTrackingAutomationApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapper(typeof(InventoryTrackingAutomationApplicationModule).Assembly);
        context.Services.AddAutoMapperObjectMapper<InventoryTrackingAutomationApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<InventoryTrackingAutomationApplicationModule>(validate: false);
        });

        // Kimlik doğrulama servisleri kaydı
        context.Services.AddScoped<IAuthAppService, AuthAppService>();

        // Hareket talebi onay servisleri kaydı
        context.Services.AddScoped<IMovementApprovalAppService, MovementApprovalAppService>();

        // Note: Domain servisleri (AuthManager, MovementApprovalManager vb) otomatik olarak ABP tarafından kaydediliyor
        
        // IHttpClientFactory için kayıt
        context.Services.AddHttpClient();
    }
}
