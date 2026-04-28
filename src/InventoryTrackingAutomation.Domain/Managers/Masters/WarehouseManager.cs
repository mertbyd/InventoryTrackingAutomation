using AutoMapper;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Models.Masters;

namespace InventoryTrackingAutomation.Managers.Masters;

/// <summary>
/// Depo domain manager'i; depo is kurallarini merkezi uygular.
/// </summary>
//işlevi: Warehouse etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class WarehouseManager : BaseManager<Warehouse>
{
    private readonly IWorkerRepository _workerRepository;    // Depo sorumlusu FK kontrolu icin.
    private readonly IMapper _mapper;                        // Model verisini entity uzerine tasir.

    /// <summary>
    /// WarehouseManager bagimliliklarini alir.
    /// </summary>
    public WarehouseManager(
        IWarehouseRepository repository,
        IWorkerRepository workerRepository,
        IMapper mapper)
        : base(repository)
    {
        _workerRepository = workerRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Yeni depo olusturur; kod tekilligi ve sorumlu calisan varligi kontrol edilir.
    /// </summary>
//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<Warehouse> CreateAsync(CreateWarehouseModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code))
        {
            await EnsureUniqueAsync(x => x.Code == model.Code);
        }

        await EnsureExistsInAsync(_workerRepository, model.ManagerWorkerId);

        var entity = new Warehouse(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

    /// <summary>
    /// Depoyu gunceller; kod tekilligi ve sorumlu calisan varligi kontrol edilir.
    /// </summary>
//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<Warehouse> UpdateAsync(Warehouse existing, UpdateWarehouseModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code) && existing.Code != model.Code)
        {
            await EnsureUniqueAsync(x => x.Code == model.Code, existing.Id);
        }

        await EnsureExistsInAsync(_workerRepository, model.ManagerWorkerId);

        _mapper.Map(model, existing);
        return existing;
    }
}
