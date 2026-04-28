using System.Threading.Tasks;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Events.Tasks;
using InventoryTrackingAutomation.Managers.Movements;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.EventHandlers.Tasks;

/// <summary>
/// Task Completed/Cancelled olunca aractaki kalan stoklar icin kontrollu iade talebi uretir.
/// </summary>
public class InventoryTaskCompletionEventHandler :
    ILocalEventHandler<InventoryTaskStatusChangedEto>,
    ITransientDependency
{
    private readonly TaskReturnRequestManager _taskReturnRequestManager;

    public InventoryTaskCompletionEventHandler(TaskReturnRequestManager taskReturnRequestManager)
    {
        _taskReturnRequestManager = taskReturnRequestManager;
    }

    [UnitOfWork]
    public virtual async Task HandleEventAsync(InventoryTaskStatusChangedEto e)
    {
        if (e.NewStatus != TaskStatusEnum.Completed && e.NewStatus != TaskStatusEnum.Cancelled)
        {
            return;
        }

        await _taskReturnRequestManager.CreateReturnRequestsForTaskAsync(
            e.TaskId,
            e.ChangedByUserId,
            e.ChangedByWorkerId);
    }
}
