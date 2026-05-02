using System;
using InventoryTrackingAutomation.Managers;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Workflows;
using InventoryTrackingAutomation.Interface.Movements;
using Volo.Abp.Domain.Services;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Movements;

/// <summary>
/// Workflow sonucu MovementRequest aggregate'ine uygulanacak nihai onay durumunu yoneten manager.
/// </summary>
public class MovementRequestWorkflowCompletionManager : InventoryTrackingAutomationDomainService
{
    public MovementRequestWorkflowCompletionManager(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IMovementRequestRepository _movementRequestRepository => LazyGetRequiredService<IMovementRequestRepository>();



    /// Workflow sonucunu hareket talebine uygulamak için kullanılır.
    public async Task ApplyWorkflowResultAsync(Guid movementRequestId, WorkflowState finalState)
    {
        var request = await _movementRequestRepository.FindAsync(movementRequestId);
        if (request == null)
        {
            return;
        }

        if (finalState == WorkflowState.Completed)
        {
            request.Status = MovementStatusEnum.Approved;
        }
        else if (finalState == WorkflowState.Rejected)
        {
            request.Status = MovementStatusEnum.Rejected;
        }

        await _movementRequestRepository.UpdateAsync(request, autoSave: true);
    }
}
