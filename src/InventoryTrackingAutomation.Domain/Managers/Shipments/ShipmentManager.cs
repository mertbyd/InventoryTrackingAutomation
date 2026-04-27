using AutoMapper;
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
    private readonly IMapper _mapper;
    public ShipmentManager(
        IShipmentRepository repository,
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
    /// Yeni sevkiyat oluşturur — ShipmentNumber unique, VehicleId ve DriverWorkerId varlık kontrolü yapar.
    /// </summary>
    public async Task<Shipment> CreateAsync(CreateShipmentModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.ShipmentNumber))
        {
            await EnsureUniqueAsync(
                x => x.ShipmentNumber == model.ShipmentNumber);
        }

        await EnsureExistsInAsync(
            _vehicleRepository,
            model.VehicleId);

        await EnsureExistsInAsync(
            _workerRepository,
            model.DriverWorkerId);

        var entity = new Shipment(GuidGenerator.Create());
        _mapper.Map(model, entity);
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
                existing.Id);
        }

        if (existing.VehicleId != model.VehicleId)
        {
            await EnsureExistsInAsync(
                _vehicleRepository,
                model.VehicleId);
        }

        if (existing.DriverWorkerId != model.DriverWorkerId)
        {
            await EnsureExistsInAsync(
                _workerRepository,
                model.DriverWorkerId);
        }

        await EnsureValidEnumAsync(model.Status, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Shipments.AllowedShipmentStatuses);

        _mapper.Map(model, existing);
        return existing;
    }
}
