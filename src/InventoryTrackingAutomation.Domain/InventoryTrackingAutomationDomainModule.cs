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
}
