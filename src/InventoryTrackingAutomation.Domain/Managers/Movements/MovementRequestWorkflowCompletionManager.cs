using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Workflows;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Managers.Stock;
using InventoryTrackingAutomation.Models.Stock;
using Volo.Abp.Domain.Services;
using Volo.Abp;

namespace InventoryTrackingAutomation.Managers.Movements;

/// <summary>
/// Workflow sonucu MovementRequest aggregate'ine uygulanacak domain aksiyonlarini yoneten manager.
/// </summary>
public class MovementRequestWorkflowCompletionManager : DomainService
{
    private readonly IMovementRequestRepository _movementRequestRepository;
    private readonly IMovementRequestLineRepository _movementRequestLineRepository;
    private readonly IVehicleTaskRepository _vehicleTaskRepository;
    private readonly StockTransferManager _stockTransferManager;

    public MovementRequestWorkflowCompletionManager(
        IMovementRequestRepository movementRequestRepository,
        IMovementRequestLineRepository movementRequestLineRepository,
        IVehicleTaskRepository vehicleTaskRepository,
        StockTransferManager stockTransferManager)
    {
        _movementRequestRepository = movementRequestRepository;
        _movementRequestLineRepository = movementRequestLineRepository;
        _vehicleTaskRepository = vehicleTaskRepository;
        _stockTransferManager = stockTransferManager;
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
            // Completed workflow talebi onaylar ve PITON stok hareketini uygular.
            var lines = await _movementRequestLineRepository.GetListAsync(x => x.MovementRequestId == request.Id);
            await ApplyApprovedTransfersAsync(request, lines);
            request.Status = MovementStatusEnum.Approved;
        }
        else if (finalState == WorkflowState.Rejected)
        {
            // Rejected workflow talebi reddedildi durumuna tasir.
            request.Status = MovementStatusEnum.Rejected;
        }

        await _movementRequestRepository.UpdateAsync(request, autoSave: true);
    }

    private async Task ApplyApprovedTransfersAsync(
        Entities.Movements.MovementRequest request,
        IReadOnlyList<Entities.Movements.MovementRequestLine> lines)
    {
        // Onaylanan talep satirlari genel stok transfer manager'i ile ledger'a islenir.
        var activeTaskId = await ResolveActiveTaskIdAsync(request);
        foreach (var line in lines)
        {
            await _stockTransferManager.ExecuteAsync(new StockTransferModel
            {
                ProductId = line.ProductId,
                Quantity = line.Quantity,
                SourceLocationType = InventoryLocationTypeEnum.Warehouse,
                SourceLocationId = request.SourceWarehouseId,
                TargetLocationType = ResolveTargetLocationType(request),
                TargetLocationId = ResolveTargetLocationId(request),
                TransactionType = ResolveTransactionType(request),
                RelatedMovementRequestId = request.Id,
                RelatedTaskId = activeTaskId,
                Note = request.RequestNumber
            });
        }
    }

    private async Task<Guid?> ResolveActiveTaskIdAsync(Entities.Movements.MovementRequest request)
    {
        if (!request.RequestedVehicleId.HasValue)
        {
            return null;
        }

        var activeVehicleTask = (await _vehicleTaskRepository.GetListAsync(x =>
                x.VehicleId == request.RequestedVehicleId.Value &&
                x.IsActive))
            .FirstOrDefault();

        if (activeVehicleTask == null)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.InventoryTransactions.InvalidTransfer)
                .WithData("VehicleId", request.RequestedVehicleId.Value);
        }

        return activeVehicleTask.InventoryTaskId;
    }

    private static InventoryLocationTypeEnum ResolveTargetLocationType(Entities.Movements.MovementRequest request)
    {
        // Arac secildiyse hedef arac stogu, aksi halde hedef depo stokudur.
        return request.RequestedVehicleId.HasValue
            ? InventoryLocationTypeEnum.Vehicle
            : InventoryLocationTypeEnum.Warehouse;
    }

    private static Guid ResolveTargetLocationId(Entities.Movements.MovementRequest request)
    {
        return request.RequestedVehicleId ?? request.TargetWarehouseId;
    }

    private static InventoryTransactionTypeEnum ResolveTransactionType(Entities.Movements.MovementRequest request)
    {
        return request.RequestedVehicleId.HasValue
            ? InventoryTransactionTypeEnum.WarehouseToVehicle
            : InventoryTransactionTypeEnum.WarehouseToWarehouse;
    }
}
