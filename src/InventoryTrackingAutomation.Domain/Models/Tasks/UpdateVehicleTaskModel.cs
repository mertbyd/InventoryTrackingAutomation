using System;

namespace InventoryTrackingAutomation.Models.Tasks;

/// <summary>
/// Arac-gorev atamasi guncelleme domain modeli.
/// </summary>
public class UpdateVehicleTaskModel
{
    public Guid VehicleId { get; set; }       // Goreve atanacak arac Id'si.
    public Guid InventoryTaskId { get; set; } // Aracin atanacagi gorev Id'si.
    public DateTime AssignedAt { get; set; }  // Atama zamani.
    public DateTime? ReleasedAt { get; set; } // Birakma zamani.
    public bool IsActive { get; set; }        // Atama aktif mi.
}
