using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Workflows;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Managers.Shipments;
using InventoryTrackingAutomation.Managers.Stock;
using Volo.Abp.Domain.Services;

namespace InventoryTrackingAutomation.Managers.Movements;

/// <summary>
/// Workflow sonucu MovementRequest aggregate'ine uygulanacak domain aksiyonlarini yoneten manager.
/// </summary>
public class MovementRequestWorkflowCompletionManager : DomainService
{
    private readonly IMovementRequestRepository _movementRequestRepository;
    private readonly IMovementRequestLineRepository _movementRequestLineRepository;
    private readonly MovementRequestStockManager _stockManager;
    private readonly MovementRequestShipmentManager _shipmentManager;

    public MovementRequestWorkflowCompletionManager(
        IMovementRequestRepository movementRequestRepository,
        IMovementRequestLineRepository movementRequestLineRepository,
        MovementRequestStockManager stockManager,
        MovementRequestShipmentManager shipmentManager)
    {
        _movementRequestRepository = movementRequestRepository;
        _movementRequestLineRepository = movementRequestLineRepository;
        _stockManager = stockManager;
        _shipmentManager = shipmentManager;
    }

    public async Task ApplyWorkflowResultAsync(Guid movementRequestId, WorkflowState finalState)
    {
        var request = await _movementRequestRepository.FindAsync(movementRequestId);
        if (request == null)
        {
            return;
        }

        if (finalState == WorkflowState.Completed)
        {
            // Completed workflow talebi onaylar, stogu dusurur ve hazirlik sevkiyati olusturur.
            var lines = await _movementRequestLineRepository.GetListAsync(x => x.MovementRequestId == request.Id);
            await _stockManager.DecreaseSourceStockAsync(request, lines);
            var shipment = await _shipmentManager.CreatePreparingShipmentAsync(request, lines);

            request.ShipmentId = shipment.Id;
            request.Status = MovementStatusEnum.Approved;
        }
        else if (finalState == WorkflowState.Rejected)
        {
            // Rejected workflow talebi reddedildi durumuna tasir.
            request.Status = MovementStatusEnum.Rejected;
        }

        await _movementRequestRepository.UpdateAsync(request, autoSave: true);
    }
}
