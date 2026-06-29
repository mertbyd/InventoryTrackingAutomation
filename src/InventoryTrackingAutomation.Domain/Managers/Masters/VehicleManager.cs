using AutoMapper;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Models.Masters;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace InventoryTrackingAutomation.Managers.Masters;

/// <summary>
/// Araç domain manager'ı — Vehicle entity'si için iş kuralları ve validasyonları.
/// </summary>
//işlevi: Vehicle etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class VehicleManager : BaseManager<Vehicle>
{
    private IRepository<VehicleType, System.Guid> _vehicleTypeRepository => LazyGetRequiredService<IRepository<VehicleType, System.Guid>>();

    /// <summary>
    /// VehicleManager constructor'ı.
    /// </summary>
    private IMapper _mapper => LazyGetRequiredService<IMapper>();
    public VehicleManager(IVehicleRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    /// <summary>
    /// Yeni araç oluşturur — PlateNumber unique kontrolü yapar.
    /// </summary>
//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<Vehicle> CreateAsync(CreateVehicleModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.PlateNumber))
        {
            await EnsureUniqueAsync(
                x => x.PlateNumber == model.PlateNumber);
        }

        // islevi: Enum yerine gelen VehicleType lookup kaydinin DB'de var oldugunu dogrular.
        await EnsureExistsInAsync(_vehicleTypeRepository, model.VehicleTypeId);

        var entity = new Vehicle(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

    /// <summary>
    /// Aracı günceller — PlateNumber unique (self hariç) kontrolü yapar.
    /// </summary>
//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<Vehicle> UpdateAsync(Vehicle existing, UpdateVehicleModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.PlateNumber) && existing.PlateNumber != model.PlateNumber)
        {
            await EnsureUniqueAsync(
                x => x.PlateNumber == model.PlateNumber,
                existing.Id);
        }

        // islevi: Enum yerine gelen VehicleType lookup kaydinin DB'de var oldugunu dogrular.
        await EnsureExistsInAsync(_vehicleTypeRepository, model.VehicleTypeId);

        _mapper.Map(model, existing);
        return existing;
    }

    /// <summary>
    /// Araci operasyon gecmisi bozulmadan pasife almak icin kullanilir.
    /// </summary>
    // islevi: Master araci silmeden gorev atamalarinda kullanima kapatir.
    // sistemdeki gorevi: VehicleTask ve stok gecmisini koruyarak soft-delete kolonlarina olan ihtiyaci kaldirir.
    public Task<Vehicle> PassivateAsync(Vehicle existing)
    {
        existing.IsActive = false;
        return Task.FromResult(existing);
    }
}

