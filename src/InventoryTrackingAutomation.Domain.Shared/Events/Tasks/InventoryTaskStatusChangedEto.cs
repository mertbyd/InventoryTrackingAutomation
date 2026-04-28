using System;
using InventoryTrackingAutomation.Enums.Tasks;

namespace InventoryTrackingAutomation.Events.Tasks;

//işlevi: InventoryTaskStatusChanged olayı (event) gerçekleştiğinde taşınacak olan veriyi tanımlar.
//sistemdeki görevi: Dağıtık sistemde veya uygulama içinde olay tabanlı iletişimi sağlar.
public class InventoryTaskStatusChangedEto
{
    public Guid TaskId { get; set; }
    public TaskStatusEnum PreviousStatus { get; set; }
    public TaskStatusEnum NewStatus { get; set; }
}
