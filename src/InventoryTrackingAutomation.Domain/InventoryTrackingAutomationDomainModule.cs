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
    // iÅŸlevi: Domain katmanÄ± modÃ¼l tanÄ±mÄ±dÄ±r.
    // sistemdeki gÃ¶revi: Domain servislerinin, manager'larÄ±n ve event handler'larÄ±n ABP konvansiyonel DI
    // mekanizmasÄ± tarafÄ±ndan otomatik olarak kaydedilmesini saÄŸlar.
    // ITransientDependency implement eden tÃ¼m sÄ±nÄ±flar (DataSeedContributor, Managers vb.)
    // ABP tarafÄ±ndan otomatik olarak DI container'a eklenir â€” manuel AddTransient gerekmez.
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Elasticsearch: typed options + tek singleton client. Indexleme/arama isini generic Elasticsearch repository'leri yapar.
        var configuration = context.Services.GetConfiguration();
        Configure<ElasticsearchOptions>(configuration.GetSection(ElasticsearchOptions.SectionName));

        context.Services.AddSingleton(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ElasticsearchOptions>>().Value;
            return new ElasticsearchClient(new ElasticsearchClientSettings(new Uri(options.Url!)));
        });
    }
}
