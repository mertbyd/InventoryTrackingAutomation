using System;

namespace InventoryTrackingAutomation.Models.Tasks;

/// <summary>
/// Bir envanter gorevine atanmis arac bilgisini temsil eder.
/// </summary>
public class TaskVehicleModel
{
    public Guid VehicleTaskId { get; set; }    // Gorev-arac atama Id'si.
    public Guid TaskId { get; set; }  // Operasyon isi Id'si.
    public Guid VehicleId { get; set; }        // Arac Id'si.
    public DateTime AssignedAt { get; set; }   // Atama zamani.
    public DateTime? ReleasedAt { get; set; }  // Ayrilma zamani.
}
