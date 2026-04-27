using System;

namespace InventoryTrackingAutomation.Dtos.Workflows;

/// <summary>
/// Bir adımdaki onaycı bilgisini temsil eden DTO.
/// </summary>
public class WorkflowHistoryApproverDto
{
    /// <summary>Onaycı kullanıcı Id'si.</summary>
    public Guid UserId { get; set; }

    /// <summary>Onaycının username'i. User bulunamazsa null.</summary>
    public string? UserName { get; set; }

    /// <summary>Onaycının ad-soyad bilgisi. User bulunamazsa null.</summary>
    public string? FullName { get; set; }

    /// <summary>Aksiyon (Pending/Approved/Rejected).</summary>
    public string ActionTaken { get; set; } = string.Empty;

    /// <summary>Onay/red sırasında girilen not.</summary>
    public string? Note { get; set; }

    /// <summary>Aksiyon tarihi. Henüz aksiyon alınmamışsa null.</summary>
    public DateTime? ActionDate { get; set; }
}
