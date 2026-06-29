using System;
using System;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Movements;

//işlevi: UpdateMovementRequest verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class UpdateMovementRequestDto
{
    /// <summary>
    /// Talep numarası. Örnek: &quot;MR-2024-00123&quot;
    /// </summary>
    public string RequestNumber { get; set; }             // Talep numarası. Örnek: "MR-2024-00123"
    /// <summary>
    /// Hareketin baglanacagi arac-gorev atamasi Id.
    /// </summary>
    public Guid VehicleTaskId { get; set; }               // Hareketin baglanacagi arac-gorev atamasi Id.
    /// <summary>
    /// Öncelik. Örnek: MovementPriorityEnum.Normal
    /// </summary>
    public MovementPriorityEnum Priority { get; set; }    // Öncelik. Örnek: MovementPriorityEnum.Normal
    /// <summary>
    /// Talep gerekçesi.
    /// </summary>
    public string RequestNote { get; set; }               // Talep gerekçesi.
    /// <summary>
    /// Planlanan teslim tarihi.
    /// </summary>
    public DateTime PlannedDate { get; set; }             // Planlanan teslim tarihi.
}
