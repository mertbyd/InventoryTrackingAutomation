using System;

namespace InventoryTrackingAutomation.Dtos.Tasks;

/// <summary>
/// Arac-gorev atamasi guncelleme request DTO'su.
/// </summary>
//işlevi: UpdateVehicleTask verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class UpdateVehicleTaskDto
{
    public Guid VehicleId { get; set; }       // Arac Id'si.
    public Guid InventoryTaskId { get; set; } // Gorev Id'si.
    public DateTime AssignedAt { get; set; }  // Atama zamani.
    public DateTime? ReleasedAt { get; set; } // Birakma zamani.
    public bool IsActive { get; set; }        // Aktiflik bilgisi.
}
