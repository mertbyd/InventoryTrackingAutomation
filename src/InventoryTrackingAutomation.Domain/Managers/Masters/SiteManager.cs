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
    public SiteManager(
        ISiteRepository repository,
        IVehicleRepository vehicleRepository,
        IWorkerRepository workerRepository)
        : base(repository)
    {
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
                x => x.Code == model.Code,
                InventoryTrackingAutomationDomainErrorCodes.Sites.CodeNotUnique);
        }

        await EnsureExistsInAsync(
            _vehicleRepository,
            model.LinkedVehicleId,
            InventoryTrackingAutomationDomainErrorCodes.Vehicles.NotFound);

        await EnsureExistsInAsync(
            _workerRepository,
            model.LinkedWorkerId,
            InventoryTrackingAutomationDomainErrorCodes.Workers.NotFound);

        await EnsureValidEnumAsync(model.SiteType, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Masters.AllowedSiteTypes);

        return MapAndAssignId(model);
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
                existing.Id,
                InventoryTrackingAutomationDomainErrorCodes.Sites.CodeNotUnique);
        }

        await EnsureExistsInAsync(
            _vehicleRepository,
            model.LinkedVehicleId,
            InventoryTrackingAutomationDomainErrorCodes.Vehicles.NotFound);

        await EnsureExistsInAsync(
            _workerRepository,
            model.LinkedWorkerId,
            InventoryTrackingAutomationDomainErrorCodes.Workers.NotFound);

        await EnsureValidEnumAsync(model.SiteType, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Masters.AllowedSiteTypes);

        return MapForUpdate(model, existing);
    }
}
