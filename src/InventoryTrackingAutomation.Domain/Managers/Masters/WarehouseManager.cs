using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Models.Masters;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Masters;

/// <summary>
/// Depo domain manager'i; depo is kurallarini merkezi uygular.
/// </summary>
//işlevi: Warehouse etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class WarehouseManager : BaseManager<Warehouse>
{
    protected override string AlreadyExistsErrorCode => WarehouseExceptionCodes.AlreadyExists;

    private IWorkerRepository _workerRepository => LazyGetRequiredService<IWorkerRepository>();    // Depo sorumlusu FK kontrolu icin.

    /// <summary>
    /// WarehouseManager bagimliliklarini alir.
    /// </summary>
    public WarehouseManager(IWarehouseRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    /// <summary>
    /// Yeni depo olusturur; kod tekilligi ve sorumlu calisan varligi kontrol edilir.
    /// </summary>
    //işlevi: Etki alanı kuralını veya validasyonunu işletir.
    //sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<CreateWarehouseModel> CreateAsync(CreateWarehouseModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code))
        {
            await EnsureUniqueAsync(x => x.Code == model.Code);
        }

        await EnsureExistsInAsync(_workerRepository, model.ManagerWorkerId);

        return model;
    }

    /// <summary>
    /// Birden fazla depo oluşturur; toplu kod tekilliği ve sorumlu çalışan varlığı kontrol edilir.
    /// </summary>
    public async Task<System.Collections.Generic.List<CreateWarehouseModel>> CreateManyAsync(System.Collections.Generic.List<CreateWarehouseModel> models)
    {
        var codes = models.Where(x => !string.IsNullOrWhiteSpace(x.Code)).Select(x => x.Code).ToList();
        if (codes.Any())
        {
            await EnsureUniqueBulkAsync(codes, x => x.Code);
        }

        var managerIds = models.Where(x => x.ManagerWorkerId.HasValue).Select(x => x.ManagerWorkerId.Value).ToList();
        if (managerIds.Any())
        {
            await EnsureAllExistInAsync(_workerRepository, managerIds);
        }

        return models;
    }

    /// <summary>
    /// Depoyu gunceller; kod tekilligi ve sorumlu calisan varligi kontrol edilir.
    /// </summary>
    //işlevi: Etki alanı kuralını veya validasyonunu işletir.
    //sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<UpdateWarehouseModel> UpdateAsync(Warehouse existing, UpdateWarehouseModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code) && existing.Code != model.Code)
        {
            await EnsureUniqueAsync(x => x.Code == model.Code, existing.Id);
        }

        await EnsureExistsInAsync(_workerRepository, model.ManagerWorkerId);

        return model;
    }

    /// <summary>
    /// Depoyu operasyon gecmisi bozulmadan pasife almak icin kullanilir.
    /// </summary>
    // islevi: Master depo kaydini silmeden stok ve gorevlerde kullanima kapatir.
    // sistemdeki gorevi: Depo baglantili stok/hareket gecmisini koruyarak soft-delete kolonlarina olan ihtiyaci kaldirir.
    public Task<Warehouse> PassivateAsync(Warehouse existing)
    {
        existing.IsActive = false;
        return Task.FromResult(existing);
    }
}
