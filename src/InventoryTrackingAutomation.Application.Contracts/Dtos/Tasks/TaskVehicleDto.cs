using System;

namespace InventoryTrackingAutomation.Dtos.Tasks;

//işlevi: TaskVehicle verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class TaskVehicleDto
{
    /// <summary>
    /// Gorev-arac atama Id&apos;si.
    /// </summary>
    public Guid VehicleTaskId { get; set; }   // Gorev-arac atama Id'si.
    /// <summary>
    /// Operasyon isi Id&apos;si.
    /// </summary>
    public Guid TaskId { get; set; } // Operasyon isi Id'si.
    /// <summary>
    /// Arac Id&apos;si.
    /// </summary>
    public Guid VehicleId { get; set; }       // Arac Id'si.
    /// <summary>
    /// Atama zamani.
    /// </summary>
    public DateTime AssignedAt { get; set; }  // Atama zamani.
    /// <summary>
    /// Ayrilma zamani.
    /// </summary>
    public DateTime? ReleasedAt { get; set; } // Ayrilma zamani.

    public string VehicleTaskName { get; set; }
    public string TaskName { get; set; }
    public string VehicleName { get; set; }
}

