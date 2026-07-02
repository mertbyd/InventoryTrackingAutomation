using System;
using Elastic.Clients.Elasticsearch;
using InventoryTrackingAutomation.Search;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Identity;

namespace InventoryTrackingAutomation;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(InventoryTrackingAutomationDomainSharedModule),
    typeof(AbpPermissionManagementDomainModule),
    typeof(AbpIdentityDomainModule)
)]
public class InventoryTrackingAutomationDomainModule : AbpModule
{
    // işlevi: Domain katmanı modül tanımıdır.
    // sistemdeki görevi: Domain servislerinin, manager'ların ve event handler'ların ABP konvansiyonel DI
    // mekanizması tarafından otomatik olarak kaydedilmesini sağlar.
    // ITransientDependency implement eden tüm sınıflar (DataSeedContributor, Managers vb.)
    // ABP tarafından otomatik olarak DI container'a eklenir — manuel AddTransient gerekmez.
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Elasticsearch: typed options + tek singleton client. Indexleme/arama isini ilgili Search manager'lari yapar.
        var configuration = context.Services.GetConfiguration();
        Configure<ElasticsearchOptions>(configuration.GetSection(ElasticsearchOptions.SectionName));

        context.Services.AddSingleton(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ElasticsearchOptions>>().Value;
            return new ElasticsearchClient(new ElasticsearchClientSettings(new Uri(options.Url!)));
        });
    }
}
