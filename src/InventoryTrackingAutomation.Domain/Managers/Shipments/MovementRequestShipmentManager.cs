using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Entities.Shipments;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Shipments;
using InventoryTrackingAutomation.Shipments;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
namespace InventoryTrackingAutomation.Managers.Shipments;
/// <summary>
/// Onaylanan hareket talebinden sevkiyat olusturma kurallarini yoneten domain manager.
/// </summary>
public class MovementRequestShipmentManager : DomainService
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IShipmentLineRepository _shipmentLineRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IWorkerRepository _workerRepository;
    private readonly IGuidGenerator _guidGenerator;
    public MovementRequestShipmentManager(
        IShipmentRepository shipmentRepository,
        IShipmentLineRepository shipmentLineRepository,
        IVehicleRepository vehicleRepository,
        IWorkerRepository workerRepository,
        IGuidGenerator guidGenerator)
    {
        _shipmentRepository = shipmentRepository;
        _shipmentLineRepository = shipmentLineRepository;
        _vehicleRepository = vehicleRepository;
        _workerRepository = workerRepository;
        _guidGenerator = guidGenerator;
    }
    public async Task<Shipment> CreatePreparingShipmentAsync(MovementRequest request, IReadOnlyList<MovementRequestLine> lines)
    {
        if (!request.RequestedVehicleId.HasValue)
        {
            // Sevkiyat icin arac secimi talep aninda zorunludur.
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.Shipments.RequestedVehicleRequired);
        }
        var vehicle = await _vehicleRepository.FindAsync(request.RequestedVehicleId.Value);
        if (vehicle == null || !vehicle.IsActive)
        {
            // Pasif veya bulunamayan arac sevkiyata atanamaz.
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.Shipments.RequestedVehicleUnavailable)
                .WithData("VehicleId", request.RequestedVehicleId.Value);
        }
        await EnsureVehicleHasNoActiveShipmentAsync(vehicle.Id);
        var driver = (await _workerRepository.GetListAsync(x => x.WorkerType == WorkerTypeEnum.BlueCollar)).FirstOrDefault();
        if (driver == null)
        {
            // Surucu bulunamazsa talep onayi sessizce sevkiyatsiz birakilmaz.
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.Shipments.DriverNotFound);
        }
        var shipment = new Shipment(_guidGenerator.Create())
        {
            ShipmentNumber = $"{ShipmentNumberPrefixes.ApprovedMovementRequest}-{request.RequestNumber}",
            VehicleId = vehicle.Id,
            DriverWorkerId = driver.Id,
            Status = ShipmentStatusEnum.Preparing,
            PlannedDepartureTime = DateTime.UtcNow.AddHours(ShipmentPlanningDefaults.PlannedDepartureHours)
        };
        await _shipmentRepository.InsertAsync(shipment, autoSave: true);
        await CreateShipmentLinesAsync(shipment.Id, lines);
        return shipment;
    }

    private async Task EnsureVehicleHasNoActiveShipmentAsync(Guid vehicleId)
    {
        // Ayni arac hazirlanan veya yolda olan baska bir sevkiyatta kullanilamaz.
        var activeShipments = await _shipmentRepository.GetListAsync(x =>
            x.VehicleId == vehicleId &&
            (x.Status == ShipmentStatusEnum.Preparing || x.Status == ShipmentStatusEnum.InTransit));

        if (activeShipments.Any())
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.Shipments.RequestedVehicleUnavailable)
                .WithData("VehicleId", vehicleId);
        }
    }

    private async Task CreateShipmentLinesAsync(Guid shipmentId, IReadOnlyList<MovementRequestLine> lines)
    {
        foreach (var line in lines)
        {
            // Sevkiyat satiri hareket talebi satirinin fiziksel tasima karsiligidir.
            var shipmentLine = new ShipmentLine(_guidGenerator.Create())
            {
                ShipmentId = shipmentId,
                MovementRequestLineId = line.Id,
                ProductId = line.ProductId,
                Quantity = line.Quantity
            };

            await _shipmentLineRepository.InsertAsync(shipmentLine, autoSave: true);
        }
    }
}
