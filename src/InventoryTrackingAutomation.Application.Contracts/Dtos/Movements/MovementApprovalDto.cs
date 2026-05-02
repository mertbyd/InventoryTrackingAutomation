using System;

namespace InventoryTrackingAutomation.Dtos.Movements;

//işlevi: MovementApproval verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class MovementApprovalDto
{
    /// <summary>
    /// Id alanı.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// MovementRequestId alanı.
    /// </summary>
    public Guid MovementRequestId { get; set; }
    /// <summary>
    /// ApproverWorkerId alanı.
    /// </summary>
    public Guid ApproverWorkerId { get; set; }
    /// <summary>
    /// StepOrder alanı.
    /// </summary>
    public int StepOrder { get; set; }
    /// <summary>
    /// Status alanı.
    /// </summary>
    public string Status { get; set; }
    /// <summary>
    /// DecidedAt alanı.
    /// </summary>
    public DateTime? DecidedAt { get; set; }
    /// <summary>
    /// Note alanı.
    /// </summary>
    public string Note { get; set; }
}
