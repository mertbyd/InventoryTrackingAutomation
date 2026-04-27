using System;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Movements;

/// <summary>
/// Hareket talebi oluşturma request DTO'su.
/// RequestedByWorkerId ve WorkflowInstanceId server-side çözümlenir, client'tan alınmaz.
/// </summary>
public class CreateMovementRequestDto
{
    public string RequestNumber { get; set; }             // Talep numarası. Örnek: "MR-2024-00123"
    public Guid SourceSiteId { get; set; }                // Kaynak lokasyon Id.
    public Guid TargetSiteId { get; set; }                // Hedef lokasyon Id.
    public Guid RequestedVehicleId { get; set; }           // Talep edilen sevkiyat aracı Id.
    public MovementPriorityEnum Priority { get; set; }    // Öncelik. Örnek: MovementPriorityEnum.Normal
    public string RequestNote { get; set; }               // Talep gerekçesi.
    public DateTime PlannedDate { get; set; }             // Planlanan teslim tarihi.
}
