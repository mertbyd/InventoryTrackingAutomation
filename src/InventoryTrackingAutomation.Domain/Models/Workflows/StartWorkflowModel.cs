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
    
    // Domain katmanına geçerken eklenecek olan Context verileri
    public Guid InitiatorUserId { get; set; }
    public Guid? InitiatorsManagerUserId { get; set; }
}
