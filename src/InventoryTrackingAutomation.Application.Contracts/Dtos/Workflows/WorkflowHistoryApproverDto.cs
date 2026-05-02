using System;

namespace InventoryTrackingAutomation.Dtos.Workflows;

//işlevi: WorkflowHistoryApprover verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class WorkflowHistoryApproverDto
{
    /// <summary>
    /// UserId alanı.
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// UserName alanı.
    /// </summary>
    public string? UserName { get; set; }
    /// <summary>
    /// FullName alanı.
    /// </summary>
    public string? FullName { get; set; }

    public string ActionTaken { get; set; } = string.Empty;
    /// <summary>
    /// Note alanı.
    /// </summary>
    public string? Note { get; set; }
    /// <summary>
    /// ActionDate alanı.
    /// </summary>
    public DateTime? ActionDate { get; set; }
}
