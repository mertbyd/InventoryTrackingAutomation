using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Models.Masters;

namespace InventoryTrackingAutomation.Managers.Masters;

/// <summary>
/// Araç domain manager'ı — Vehicle entity'si için iş kuralları ve validasyonları.
/// </summary>
public class VehicleManager : BaseManager<Vehicle>
{
    /// <summary>
    /// VehicleManager constructor'ı.
    /// </summary>
    public VehicleManager(
        IVehicleRepository repository)
        : base(repository)
    {
    }

    /// <summary>
    /// Yeni araç oluşturur — PlateNumber unique kontrolü yapar.
    /// </summary>
    public async Task<Vehicle> CreateAsync(CreateVehicleModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.PlateNumber))
        {
            await EnsureUniqueAsync(
                x => x.PlateNumber == model.PlateNumber,
                InventoryTrackingAutomationDomainErrorCodes.Vehicles.PlateNumberNotUnique);
        }

        await EnsureValidEnumAsync(model.VehicleType, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Masters.AllowedVehicleTypes);

        return MapAndAssignId(model);
    }

    /// <summary>
    /// Aracı günceller — PlateNumber unique (self hariç) kontrolü yapar.
    /// </summary>
    public async Task<Vehicle> UpdateAsync(Vehicle existing, UpdateVehicleModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.PlateNumber) && existing.PlateNumber != model.PlateNumber)
        {
            await EnsureUniqueAsync(
                x => x.PlateNumber == model.PlateNumber,
                existing.Id,
                InventoryTrackingAutomationDomainErrorCodes.Vehicles.PlateNumberNotUnique);
        }

        await EnsureValidEnumAsync(model.VehicleType, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Masters.AllowedVehicleTypes);

        return MapForUpdate(model, existing);
    }
}
