using System;
using System.Collections.Generic;

namespace InventoryTrackingAutomation.Models.Workflows;

/// <summary>
/// Workflow onayı işlenirken kullanılan Domain Model.
/// </summary>
public class ProcessApprovalModel
{
    public Guid InstanceStepId { get; set; }
    public bool IsApproved { get; set; }
    public string? Note { get; set; }
    
    // Domain katmanına geçerken eklenecek olan Context verileri
    public Guid CurrentUserId { get; set; }
    public List<string> CurrentUserRoles { get; set; } = new();
}
