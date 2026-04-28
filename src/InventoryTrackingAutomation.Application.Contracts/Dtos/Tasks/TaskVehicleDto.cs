using System;

namespace InventoryTrackingAutomation.Dtos.Tasks;

/// <summary>
/// Goreve atanmis arac bilgisini donduren DTO.
/// </summary>
public class TaskVehicleDto
{
    public Guid VehicleTaskId { get; set; }   // Gorev-arac atama Id'si.
    public Guid InventoryTaskId { get; set; } // Gorev Id'si.
    public Guid VehicleId { get; set; }       // Arac Id'si.
    public DateTime AssignedAt { get; set; }  // Atama zamani.
    public DateTime? ReleasedAt { get; set; } // Ayrilma zamani.
    public bool IsActive { get; set; }        // Aktiflik bilgisi.
}
