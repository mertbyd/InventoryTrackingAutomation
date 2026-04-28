using System;
using System.Collections.Generic;

namespace InventoryTrackingAutomation.Dtos.Workflows;

/// <summary>
/// Bir iş akışı adımını (geçmiş, mevcut veya gelecek) temsil eden DTO.
/// </summary>
//işlevi: WorkflowHistoryStep verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class WorkflowHistoryStepDto
{
    /// <summary>Adım sırası (1, 2, 3...).</summary>
    public int StepOrder { get; set; }

    /// <summary>Adım adı — RequiredRoleName veya ResolverKey'den türetilir.</summary>
    public string StepName { get; set; } = string.Empty;

    /// <summary>Rol bazlı yetki gerekiyorsa adı (Örn: "DepartmentManager").</summary>
    public string? RequiredRoleName { get; set; }

    /// <summary>Dinamik onaycı çözümleme anahtarı (Örn: "InitiatorManager").</summary>
    public string? ResolverKey { get; set; }

    /// <summary>
    /// Adım durumu:
    /// "NotStarted" — instance step henüz yaratılmamış (gelecek adım)
    /// "Pending" — instance step var, onay bekliyor
    /// "Approved" — onaylandı
    /// "Rejected" — reddedildi
    /// </summary>
    public string StepStatus { get; set; } = string.Empty;

    /// <summary>
    /// Instance step'in oluşturulma tarihi. NotStarted ise null.
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Bu adımdaki onaycı(lar). NotStarted ise boş liste.
    /// Mevcut model single-approver — listenin amacı çoklu onaycı modeline ileride uyum sağlamak.
    /// </summary>
    public List<WorkflowHistoryApproverDto> Approvers { get; set; } = new();
}
