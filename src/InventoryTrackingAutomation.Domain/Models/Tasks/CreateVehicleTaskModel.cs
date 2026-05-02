using System;

namespace InventoryTrackingAutomation.Models.Tasks;

/// <summary>
/// Arac-gorev atamasi olusturma domain modeli.
/// </summary>
public class CreateVehicleTaskModel
{
    public Guid VehicleId { get; set; }       // Goreve atanacak arac Id'si.
    public Guid TaskId { get; set; } // Aracin atanacagi operasyon isi Id'si.
    public Guid ResponsibleWorkerId { get; set; } // Atamadan sorumlu calisan Id'si.
    public DateTime AssignedAt { get; set; }  // Atama zamani.
    public DateTime? ReleasedAt { get; set; } // Birakma zamani.
}
