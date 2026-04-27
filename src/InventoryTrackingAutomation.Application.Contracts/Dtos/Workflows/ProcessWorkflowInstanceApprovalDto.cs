namespace InventoryTrackingAutomation.Dtos.Workflows;

/// <summary>
/// Bir iş akışı instance'ındaki mevcut bekleyen adımı onaylamak veya reddetmek için kullanılan DTO.
/// </summary>
public class ProcessWorkflowInstanceApprovalDto
{
    /// <summary>
    /// Onaylandı mı? (True: Approved, False: Rejected)
    /// </summary>
    public bool IsApproved { get; set; }

    /// <summary>
    /// Onay/Red notu (Opsiyonel).
    /// </summary>
    public string? Note { get; set; }
}
