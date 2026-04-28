using AutoMapper;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Models.Masters;

namespace InventoryTrackingAutomation.Managers.Masters;

/// <summary>
/// Araç domain manager'ı — Vehicle entity'si için iş kuralları ve validasyonları.
/// </summary>
//işlevi: Vehicle etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class VehicleManager : BaseManager<Vehicle>
{
    /// <summary>
    /// VehicleManager constructor'ı.
    /// </summary>
    private readonly IMapper _mapper;
    public VehicleManager(
        IVehicleRepository repository,
        IMapper mapper)
        : base(repository)
    {
        _mapper = mapper;
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

        await EnsureValidEnumAsync(model.VehicleType, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Masters.AllowedVehicleTypes);

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

        await EnsureValidEnumAsync(model.VehicleType, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Masters.AllowedVehicleTypes);

        _mapper.Map(model, existing);
        return existing;
    }
}

