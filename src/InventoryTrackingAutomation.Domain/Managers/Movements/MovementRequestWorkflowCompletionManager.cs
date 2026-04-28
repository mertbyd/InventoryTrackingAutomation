using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Workflows;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Managers.Stock;
using Volo.Abp.Domain.Services;

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
    private readonly MovementRequestStockManager _stockManager;

    public MovementRequestWorkflowCompletionManager(
        IMovementRequestRepository movementRequestRepository,
        IMovementRequestLineRepository movementRequestLineRepository,
        MovementRequestStockManager stockManager)
    {
        _movementRequestRepository = movementRequestRepository;
        _movementRequestLineRepository = movementRequestLineRepository;
        _stockManager = stockManager;
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
            await _stockManager.ApplyApprovedTransferAsync(request, lines);
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
