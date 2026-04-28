using System;
using InventoryTrackingAutomation.Enums.Workflows;

namespace InventoryTrackingAutomation.Events.Workflows;

/// <summary>
/// İş akışındaki bir adım (step) işlem gördüğünde fırlatılan event nesnesi.
/// (Örn: Yönetici veya görevli onayladı/reddetti)
/// </summary>
//işlevi: WorkflowStepProcessed olayı (event) gerçekleştiğinde taşınacak olan veriyi tanımlar.
//sistemdeki görevi: Dağıtık sistemde veya uygulama içinde olay tabanlı iletişimi sağlar.
public class WorkflowStepProcessedEto
{
    public Guid WorkflowInstanceId { get; set; }
    public Guid WorkflowInstanceStepId { get; set; }
    public Guid ProcessedByUserId { get; set; }
    public WorkflowActionType ActionTaken { get; set; }
    public string Note { get; set; }
}
