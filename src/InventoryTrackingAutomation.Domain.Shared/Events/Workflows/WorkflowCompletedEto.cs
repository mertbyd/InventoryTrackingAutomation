using System;
using InventoryTrackingAutomation.Enums.Workflows;

namespace InventoryTrackingAutomation.Events.Workflows;

/// <summary>
/// Bir iş akışı nihai duruma (Completed veya Rejected) ulaştığında fırlatılan Event Transfer Object.
/// </summary>
[Serializable]
//işlevi: WorkflowCompleted olayı (event) gerçekleştiğinde taşınacak olan veriyi tanımlar.
//sistemdeki görevi: Dağıtık sistemde veya uygulama içinde olay tabanlı iletişimi sağlar.
public class WorkflowCompletedEto
{
    public Guid WorkflowInstanceId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public WorkflowState FinalState { get; set; } // Completed veya Rejected
}
