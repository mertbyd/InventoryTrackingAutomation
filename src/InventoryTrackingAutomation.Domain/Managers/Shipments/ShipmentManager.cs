using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Shipments;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Shipments;
using InventoryTrackingAutomation.Models.Shipments;

namespace InventoryTrackingAutomation.Managers.Shipments;

/// <summary>
/// Sevkiyat domain manager'ı — Shipment entity'si için iş kuralları ve validasyonları.
/// </summary>
public class ShipmentManager : BaseManager<Shipment>
{
    private readonly IVehicleRepository _vehicleRepository;  // VehicleId FK validasyonu için
    private readonly IWorkerRepository _workerRepository;    // DriverWorkerId FK validasyonu için

    /// <summary>
    /// ShipmentManager constructor'ı.
    /// </summary>
    public ShipmentManager(
        IShipmentRepository repository,
        IVehicleRepository vehicleRepository,
        IWorkerRepository workerRepository)
        : base(repository)
    {
        _vehicleRepository = vehicleRepository;
        _workerRepository = workerRepository;
    }

    /// <summary>
    /// Yeni sevkiyat oluşturur — ShipmentNumber unique, VehicleId ve DriverWorkerId varlık kontrolü yapar.
    /// </summary>
    public async Task<Shipment> CreateAsync(CreateShipmentModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.ShipmentNumber))
        {
            await EnsureUniqueAsync(
                x => x.ShipmentNumber == model.ShipmentNumber,
                InventoryTrackingAutomationDomainErrorCodes.Shipments.ShipmentNumberNotUnique);
        }

        await EnsureExistsInAsync(
            _vehicleRepository,
            model.VehicleId,
            InventoryTrackingAutomationDomainErrorCodes.Vehicles.NotFound);

        await EnsureExistsInAsync(
            _workerRepository,
            model.DriverWorkerId,
            InventoryTrackingAutomationDomainErrorCodes.Workers.NotFound);

        var entity = MapAndAssignId(model);
        entity.Status = ShipmentStatusEnum.Preparing;  // Yeni sevkiyat her zaman Preparing başlar
        return entity;
    }

    /// <summary>
    /// Sevkiyatı günceller — ShipmentNumber unique (self hariç), VehicleId ve DriverWorkerId varlık kontrolleri yapar.
    /// </summary>
    public async Task<Shipment> UpdateAsync(Shipment existing, UpdateShipmentModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.ShipmentNumber) && existing.ShipmentNumber != model.ShipmentNumber)
        {
            await EnsureUniqueAsync(
                x => x.ShipmentNumber == model.ShipmentNumber,
                existing.Id,
                InventoryTrackingAutomationDomainErrorCodes.Shipments.ShipmentNumberNotUnique);
        }

        if (existing.VehicleId != model.VehicleId)
        {
            await EnsureExistsInAsync(
                _vehicleRepository,
                model.VehicleId,
                InventoryTrackingAutomationDomainErrorCodes.Vehicles.NotFound);
        }

        if (existing.DriverWorkerId != model.DriverWorkerId)
        {
            await EnsureExistsInAsync(
                _workerRepository,
                model.DriverWorkerId,
                InventoryTrackingAutomationDomainErrorCodes.Workers.NotFound);
        }

        await EnsureValidEnumAsync(model.Status, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Shipments.AllowedShipmentStatuses);

        return MapForUpdate(model, existing);
    }
}
