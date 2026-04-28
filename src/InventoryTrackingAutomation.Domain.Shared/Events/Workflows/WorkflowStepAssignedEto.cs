using System;

namespace InventoryTrackingAutomation.Events.Workflows;

/// <summary>
/// Bir is akisi adimi onayciya atandiginda firlatilan Event Transfer Object.
/// </summary>
[Serializable]
//işlevi: WorkflowStepAssigned olayı (event) gerçekleştiğinde taşınacak olan veriyi tanımlar.
//sistemdeki görevi: Dağıtık sistemde veya uygulama içinde olay tabanlı iletişimi sağlar.
public class WorkflowStepAssignedEto
{
    public Guid WorkflowInstanceId { get; set; }
    public Guid WorkflowInstanceStepId { get; set; }
    public Guid WorkflowStepDefinitionId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public Guid? AssignedUserId { get; set; }
}
