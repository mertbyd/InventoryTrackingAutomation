using System;

namespace InventoryTrackingAutomation.Models.Workflows;

/// <summary>
/// Workflow başlatılırken kullanılan Domain Model.
/// </summary>
public class StartWorkflowModel
{
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public Guid WorkflowDefinitionId { get; set; }

    // İş akışını başlatan kullanıcının Id'si — InitiatorManager strategy'sinde yönetici çözümlemesi için.
    public Guid InitiatorUserId { get; set; }
}
