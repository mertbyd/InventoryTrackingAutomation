using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Workflows;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Managers.Inventory;
using InventoryTrackingAutomation.Managers.Tasks;
using InventoryTrackingAutomation.Models.Inventory;
using InventoryTrackingAutomation.Entities.Movements;
using Volo.Abp.Domain.Services;
using Volo.Abp;

namespace InventoryTrackingAutomation.Managers.Movements;

/// <summary>
/// Workflow sonucu MovementRequest aggregate'ine uygulanacak domain aksiyonlarini yoneten manager.
/// </summary>
//işlevi: Workflow (onay süreci) tamamlandığında veya reddedildiğinde MovementRequest üzerinde nihai durumu ve stok hareketlerini tetikler.
//sistemdeki görevii: Onay zincirinin son halkasıdır; başarılı onay sonrası stok transferini başlatır ve talebi kapatır.
public class MovementRequestWorkflowCompletionManager : DomainService
{
    private readonly IMovementRequestRepository _movementRequestRepository;
    private readonly IMovementRequestLineRepository _movementRequestLineRepository;
    private readonly StockTransferManager _stockTransferManager;
    private readonly VehicleTaskManager _vehicleTaskManager;

    public MovementRequestWorkflowCompletionManager(
        IMovementRequestRepository movementRequestRepository,
        IMovementRequestLineRepository movementRequestLineRepository,
        StockTransferManager stockTransferManager,
        VehicleTaskManager vehicleTaskManager)
    {
        _movementRequestRepository = movementRequestRepository;
        _movementRequestLineRepository = movementRequestLineRepository;
        _stockTransferManager = stockTransferManager;
        _vehicleTaskManager = vehicleTaskManager;
    }

    public async Task ApplyWorkflowResultAsync(Guid movementRequestId, WorkflowState finalState)
    {
        var request = await _movementRequestRepository.FindAsync(movementRequestId);
        if (request == null) return;

        if (finalState == WorkflowState.Completed)
        {
            var lines = await _movementRequestLineRepository.GetListAsync(x => x.MovementRequestId == request.Id);

            // Her satir icin atomik transfer + ledger.
            foreach (var line in lines)
                await _stockTransferManager.ExecuteAsync(BuildTransferModel(request, line));

            // Gorev baglami varsa: VehicleTask kaydini garanti et.
            if (request.AssignedTaskId.HasValue && request.RequestedVehicleId.HasValue)
            {
                // PITON planı: ResolveDriverForRequest basit bir implementasyon gerektirir veya modelden alınır.
                // Burada VehicleTask'ın atanmasını sağlıyoruz. EnsureAssignedAsync metodu TaskManager'da olacak.
                await _vehicleTaskManager.EnsureAssignedAsync(
                    request.AssignedTaskId.Value,
                    request.RequestedVehicleId.Value,
                    request.RequestedByWorkerId); // Varsayılan olarak talep edeni driver kabul ettik (plan detay vermemiş)
            }

            request.Status = MovementStatusEnum.Approved;
        }
        else if (finalState == WorkflowState.Rejected)
        {
            request.Status = MovementStatusEnum.Rejected;
        }

        await _movementRequestRepository.UpdateAsync(request, autoSave: true);
    }

    private StockTransferModel BuildTransferModel(MovementRequest req, MovementRequestLine line)
    {
        var hasTaskContext = req.AssignedTaskId.HasValue && req.RequestedVehicleId.HasValue;
        return new StockTransferModel
        {
            ProductId = line.ProductId,
            Quantity = line.Quantity,
            SourceLocationType = StockLocationTypeEnum.Warehouse,
            SourceLocationId = req.SourceWarehouseId,
            DestinationLocationType = hasTaskContext
                ? StockLocationTypeEnum.Vehicle
                : StockLocationTypeEnum.Warehouse,
            DestinationLocationId = hasTaskContext
                ? req.RequestedVehicleId!.Value
                : req.TargetWarehouseId
                  ?? throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequests.TargetRequired),
            TransactionType = hasTaskContext
                ? InventoryTransactionTypeEnum.WarehouseToVehicle
                : InventoryTransactionTypeEnum.WarehouseToWarehouse,
            RelatedMovementRequestId = req.Id,
            RelatedTaskId = req.AssignedTaskId,
            PerformedByUserId = null
        };
    }
}
