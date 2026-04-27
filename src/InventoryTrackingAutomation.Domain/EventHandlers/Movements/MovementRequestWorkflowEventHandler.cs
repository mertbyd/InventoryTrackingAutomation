using System.Threading.Tasks;
using InventoryTrackingAutomation.Events.Workflows;
using InventoryTrackingAutomation.Managers.Movements;
using InventoryTrackingAutomation.Workflows;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace InventoryTrackingAutomation.EventHandlers.Movements;

/// <summary>
/// Workflow sonucu MovementRequest domain aksiyonunu baslatan event handler.
/// </summary>
public class MovementRequestWorkflowEventHandler : ILocalEventHandler<WorkflowCompletedEto>, ITransientDependency
{
    private readonly MovementRequestWorkflowCompletionManager _completionManager;

    public MovementRequestWorkflowEventHandler(MovementRequestWorkflowCompletionManager completionManager)
    {
        _completionManager = completionManager;
    }

    [Volo.Abp.Uow.UnitOfWork]
    public virtual async Task HandleEventAsync(WorkflowCompletedEto eventData)
    {
        // Bu handler sadece MovementRequest workflow sonucunu uygular.
        if (eventData.EntityType != WorkflowEntityTypes.MovementRequest)
        {
            return;
        }

        await _completionManager.ApplyWorkflowResultAsync(eventData.EntityId, eventData.FinalState);
    }
}
