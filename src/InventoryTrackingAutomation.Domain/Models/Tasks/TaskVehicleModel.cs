using System;

namespace InventoryTrackingAutomation.Models.Tasks;

/// <summary>
/// Bir envanter gorevine atanmis arac bilgisini temsil eder.
/// </summary>
public class TaskVehicleModel
{
    public Guid VehicleTaskId { get; set; }    // Gorev-arac atama Id'si.
    public Guid InventoryTaskId { get; set; }  // Gorev Id'si.
    public Guid VehicleId { get; set; }        // Arac Id'si.
    public DateTime AssignedAt { get; set; }   // Atama zamani.
    public DateTime? ReleasedAt { get; set; }  // Ayrilma zamani.
    public bool IsActive { get; set; }         // Atama aktif mi.
}
