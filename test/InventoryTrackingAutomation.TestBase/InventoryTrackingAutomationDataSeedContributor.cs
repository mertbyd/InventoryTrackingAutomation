using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation;

/// <summary>
/// Test ortamında seed verisi için placeholder.
/// Prodüksiyondaki InventoryTrackingAutomationDataSeedContributor (Domain klasöründe)
/// tüm test verilerini sağlıyor. Test projesi kendi seed'i gerekirse burada eklenebilir.
/// </summary>
public class InventoryTrackingAutomationDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;

    public InventoryTrackingAutomationDataSeedContributor(
        IGuidGenerator guidGenerator, ICurrentTenant currentTenant)
    {
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
    }

    public Task SeedAsync(DataSeedContext context)
    {
        // Test seed'i prodüksiyondaki contributor tarafından yönetiliyor.
        // Buraya test-spesifik seed veri eklenebilir.

        using (_currentTenant.Change(context?.TenantId))
        {
            return Task.CompletedTask;
        }
    }
}
