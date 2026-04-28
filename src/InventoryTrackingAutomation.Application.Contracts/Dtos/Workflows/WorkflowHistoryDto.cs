using System;
using System.Collections.Generic;

namespace InventoryTrackingAutomation.Dtos.Workflows;

/// <summary>
/// Bir iş akışı sürecinin tam tarihçesini dönen DTO.
/// Süreci kim başlattı, hangi adımlar tamamlandı/bekliyor/henüz başlamadı bilgisini içerir.
/// </summary>
//işlevi: WorkflowHistory verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class WorkflowHistoryDto
{
    /// <summary>İş akışı süreci (instance) Id'si.</summary>
    public Guid WorkflowInstanceId { get; set; }

    /// <summary>İş akışı tanım adı (Örn: "MovementRequest").</summary>
    public string WorkflowDefinitionName { get; set; } = string.Empty;

    /// <summary>İş akışı açıklaması (Örn: "Hareket Talebi İş Akışı").</summary>
    public string? WorkflowDescription { get; set; }

    /// <summary>Bağlı entity türü (Örn: "MovementRequest").</summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>Bağlı entity Id'si.</summary>
    public Guid EntityId { get; set; }

    /// <summary>İş akışının anlık durumu (Active/Completed/Rejected).</summary>
    public string State { get; set; } = string.Empty;

    /// <summary>Süreci başlatan kullanıcı Id'si.</summary>
    public Guid InitiatorUserId { get; set; }

    /// <summary>Süreci başlatan kullanıcının username'i.</summary>
    public string? InitiatorUserName { get; set; }

    /// <summary>Süreci başlatan kullanıcının ad-soyad bilgisi.</summary>
    public string? InitiatorFullName { get; set; }

    /// <summary>Sürecin oluşturulma tarihi.</summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>Tüm adımlar (geçmiş + mevcut + gelecek). StepOrder'a göre artan sırada.</summary>
    public List<WorkflowHistoryStepDto> Steps { get; set; } = new();
}
