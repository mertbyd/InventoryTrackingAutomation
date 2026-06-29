using System;
using System;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Models.Movements;

/// <summary>
/// Hareket talebi guncelleme domain modeli.
/// </summary>
public class UpdateMovementRequestModel
{
    public string RequestNumber { get; set; }                   // Talep numarasi. Ornek: "MR-2024-00123"
    public Guid RequestedByWorkerId { get; set; }               // Talebi olusturan calisan Id.
    public Guid VehicleTaskId { get; set; }                     // Hareketin baglanacagi arac-gorev atamasi Id.
    public MovementPriorityEnum Priority { get; set; }          // Oncelik. Ornek: MovementPriorityEnum.Normal
    public string RequestNote { get; set; }                     // Talep gerekcesi.
    public DateTime PlannedDate { get; set; }                   // Planlanan teslim tarihi.
}
