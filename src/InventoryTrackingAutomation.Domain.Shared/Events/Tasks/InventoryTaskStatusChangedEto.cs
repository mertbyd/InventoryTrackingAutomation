using System;
using InventoryTrackingAutomation.Enums.Tasks;

namespace InventoryTrackingAutomation.Events.Tasks;

public class InventoryTaskStatusChangedEto
{
    public Guid TaskId { get; set; }
    public TaskStatusEnum PreviousStatus { get; set; }
    public TaskStatusEnum NewStatus { get; set; }
}
