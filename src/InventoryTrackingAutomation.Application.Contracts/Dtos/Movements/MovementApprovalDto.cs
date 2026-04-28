using System;

namespace InventoryTrackingAutomation.Dtos.Movements;

/// <summary>
/// Hareket talebinin bir onay adımının detayları.
/// </summary>
//işlevi: MovementApproval verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class MovementApprovalDto
{
    /// <summary>
    /// Onay kaydının benzersiz kimliği.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// İlişkili hareket talebinin kimliği.
    /// </summary>
    public Guid MovementRequestId { get; set; }

    /// <summary>
    /// Onay kararını veren çalışanın kimliği.
    /// </summary>
    public Guid ApproverWorkerId { get; set; }

    /// <summary>
    /// Onay işlemi sırasındaki adım numarası (1, 2, 3...).
    /// </summary>
    public int StepOrder { get; set; }

    /// <summary>
    /// Onay durumu: Approved, Rejected.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Karar verilme tarihi/saati.
    /// </summary>
    public DateTime? DecidedAt { get; set; }

    /// <summary>
    /// Onaylayan kişinin yorumu veya reddetme nedeni.
    /// </summary>
    public string Note { get; set; }
}
