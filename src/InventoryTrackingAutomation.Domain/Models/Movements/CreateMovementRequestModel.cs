using System;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Models.Movements;

/// <summary>
/// Hareket talebi olusturma domain modeli.
/// </summary>
public class CreateMovementRequestModel
{
    public string RequestNumber { get; set; }                   // Talep numarasi. Ornek: "MR-2024-00123"
    public Guid RequestedByWorkerId { get; set; }               // Talebi olusturan calisan Id.
    public Guid SourceWarehouseId { get; set; }                      // Kaynak lokasyon Id.
    public Guid TargetWarehouseId { get; set; }                      // Hedef lokasyon Id.
    public Guid RequestedVehicleId { get; set; }                 // Talep edilen arac Id.
    public MovementStatusEnum Status { get; set; }              // Talep durumu. Ornek: MovementStatusEnum.Pending
    public MovementPriorityEnum Priority { get; set; }          // Oncelik. Ornek: MovementPriorityEnum.Normal
    public string RequestNote { get; set; }                     // Talep gerekcesi.
    public DateTime PlannedDate { get; set; }                   // Planlanan teslim tarihi.
}
