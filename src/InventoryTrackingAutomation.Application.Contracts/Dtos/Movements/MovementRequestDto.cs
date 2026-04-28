using System;
using Volo.Abp.Application.Dtos;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Movements;

/// <summary>
/// Hareket talebi response DTO'su.
/// </summary>
public class MovementRequestDto : FullAuditedEntityDto<Guid>
{
    public string RequestNumber { get; set; }             // Talep numarasi. Ornek: "MR-2024-00123"
    public Guid RequestedByWorkerId { get; set; }         // Talebi olusturan calisan Id.
    public Guid SourceSiteId { get; set; }                // Kaynak lokasyon Id.
    public Guid TargetSiteId { get; set; }                // Hedef lokasyon Id.
    public Guid? RequestedVehicleId { get; set; }          // Talep edilen arac Id.
    public MovementStatusEnum Status { get; set; }        // Talep durumu. Ornek: MovementStatusEnum.Pending
    public MovementPriorityEnum Priority { get; set; }    // Oncelik. Ornek: MovementPriorityEnum.Normal
    public string RequestNote { get; set; }               // Talep gerekcesi.
    public DateTime PlannedDate { get; set; }             // Planlanan teslim tarihi.
    public string? CancellationNote { get; set; }         // Iptal gerekcesi.
    public Guid? WorkflowInstanceId { get; set; }         // Bagli is akisi Id'si.
}
