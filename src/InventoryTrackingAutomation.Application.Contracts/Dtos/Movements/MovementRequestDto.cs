using System;
using Volo.Abp.Application.Dtos;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Movements;

/// <summary>
/// Hareket talebi response DTO'su — GetAll ve GetById operasyonlarında döner.
/// </summary>
public class MovementRequestDto : FullAuditedEntityDto<Guid>
{
    public string RequestNumber { get; set; }             // Talep numarası. Örnek: "MR-2024-00123"
    public Guid RequestedByWorkerId { get; set; }         // Talebi oluşturan çalışan Id.
    public Guid SourceSiteId { get; set; }                // Kaynak lokasyon Id.
    public Guid TargetSiteId { get; set; }                // Hedef lokasyon Id.
    public MovementStatusEnum Status { get; set; }        // Talep durumu. Örnek: MovementStatusEnum.Pending
    public MovementPriorityEnum Priority { get; set; }    // Öncelik. Örnek: MovementPriorityEnum.Normal
    public string RequestNote { get; set; }               // Talep gerekçesi.
    public DateTime PlannedDate { get; set; }             // Planlanan teslim tarihi.
    public Guid? ShipmentId { get; set; }                 // Bağlı sevkiyat Id.
    public string? CancellationNote { get; set; }         // İptal gerekçesi.
    public Guid? WorkflowInstanceId { get; set; }         // Bağlı iş akışı Id'si.
}
