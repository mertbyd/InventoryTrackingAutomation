using System;

namespace InventoryTrackingAutomation.Dtos.Workflows;

/// <summary>
/// İş akışında bir onayı veya reddi işlemek için kullanılan DTO.
/// </summary>
public class ProcessApprovalDto
{
    /// <summary>
    /// Aksiyon alınacak adımın Id'si.
    /// </summary>
    public Guid InstanceStepId { get; set; }

    /// <summary>
    /// Onaylandı mı? (True: Approved, False: Rejected)
    /// </summary>
    public bool IsApproved { get; set; }

    /// <summary>
    /// Onay/Red notu (Opsiyonel).
    /// </summary>
    public string? Note { get; set; }
}
