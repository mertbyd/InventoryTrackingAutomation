using AutoMapper;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Models.Masters;

namespace InventoryTrackingAutomation.Managers.Masters;

/// <summary>
/// Depo domain manager'i; depo is kurallarini merkezi uygular.
/// </summary>
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
