using AutoMapper;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Models.Masters;

namespace InventoryTrackingAutomation.Managers.Masters;

/// <summary>
/// Lokasyon domain manager'ı — Site entity'si için iş kuralları ve validasyonları.
/// </summary>
public class SiteManager : BaseManager<Site>
{
    private readonly IVehicleRepository _vehicleRepository;  // LinkedVehicleId FK validasyonu için
    private readonly IWorkerRepository _workerRepository;    // LinkedWorkerId FK validasyonu için

    /// <summary>
    /// SiteManager constructor'ı.
    /// </summary>
    private readonly IMapper _mapper;
    public SiteManager(
        ISiteRepository repository,
        IVehicleRepository vehicleRepository,
        IWorkerRepository workerRepository,
        IMapper mapper)
        : base(repository)
    {
        _mapper = mapper;
        _vehicleRepository = vehicleRepository;
        _workerRepository = workerRepository;
    }

    /// <summary>
    /// Yeni lokasyon oluşturur — Code unique, LinkedVehicleId ve LinkedWorkerId varlık kontrolü yapar.
    /// </summary>
    public async Task<Site> CreateAsync(CreateSiteModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code))
        {
            await EnsureUniqueAsync(
                x => x.Code == model.Code);
        }

        await EnsureExistsInAsync(
            _vehicleRepository,
            model.LinkedVehicleId);

        await EnsureExistsInAsync(
            _workerRepository,
            model.LinkedWorkerId);

        await EnsureValidEnumAsync(model.SiteType, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Masters.AllowedSiteTypes);

        var entity = new Site(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

    /// <summary>
    /// Lokasyonu günceller — Code unique (self hariç), LinkedVehicleId ve LinkedWorkerId kontrolü yapar.
    /// </summary>
    public async Task<Site> UpdateAsync(Site existing, UpdateSiteModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code) && existing.Code != model.Code)
        {
            await EnsureUniqueAsync(
                x => x.Code == model.Code,
                existing.Id);
        }

        await EnsureExistsInAsync(
            _vehicleRepository,
            model.LinkedVehicleId);

        await EnsureExistsInAsync(
            _workerRepository,
            model.LinkedWorkerId);

        await EnsureValidEnumAsync(model.SiteType, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Masters.AllowedSiteTypes);

        _mapper.Map(model, existing);
        return existing;
    }
}

