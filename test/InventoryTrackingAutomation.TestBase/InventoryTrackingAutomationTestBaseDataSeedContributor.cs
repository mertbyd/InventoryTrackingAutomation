using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation;

/// <summary>
/// Test ortaminda uygulama servislerinin ihtiyac duydugu minimum baglamsal seed verisini uretir.
/// </summary>
public class InventoryTrackingAutomationTestBaseDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private static readonly Guid TestAdminUserId = Guid.Parse("2e701e62-0953-4dd3-910b-dc6cc93ccb0d");

    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;
    private readonly IRepository<Worker, Guid> _workerRepository;

    public InventoryTrackingAutomationTestBaseDataSeedContributor(
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant,
        IRepository<Worker, Guid> workerRepository)
    {
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
        _workerRepository = workerRepository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        using (_currentTenant.Change(context?.TenantId))
        {
            // AppService taban sinifindaki ResolveCurrentWorkerIdAsync metodu CurrentUser.Id uzerinden
            // Worker tablosunu okur. Testlerde fake principal admin kullanicisini temsil ettigi icin
            // bu kullanicinin calisan karsiligini seed etmek zorundayiz.
            var existingWorker = await _workerRepository.FirstOrDefaultAsync(x => x.UserId == TestAdminUserId);
            if (existingWorker != null)
            {
                return;
            }

            // Departman, depo ve yonetici baglantilari opsiyonel oldugu icin testin ihtiyac duymadigi
            // FK kayitlarini burada uretmiyoruz. Boylece seed sadece kimlik -> calisan eslesmesini saglar.
            await _workerRepository.InsertAsync(
                new Worker(_guidGenerator.Create())
                {
                    UserId = TestAdminUserId,
                    RegistrationNumber = "TEST-ADMIN",
                    WorkerType = WorkerTypeEnum.WhiteCollar,
                    IsActive = true
                },
                autoSave: true
            );
        }
    }
}
