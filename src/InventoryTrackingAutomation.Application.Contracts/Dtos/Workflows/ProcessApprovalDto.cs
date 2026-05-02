using System;

namespace InventoryTrackingAutomation.Dtos.Workflows;

//işlevi: ProcessApproval verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class ProcessApprovalDto
{
    /// <summary>
    /// InstanceStepId alanı.
    /// </summary>
    public Guid InstanceStepId { get; set; }
    /// <summary>
    /// IsApproved alanı.
    /// </summary>
    public bool IsApproved { get; set; }
    /// <summary>
    /// Note alanı.
    /// </summary>
    public string? Note { get; set; }
}
