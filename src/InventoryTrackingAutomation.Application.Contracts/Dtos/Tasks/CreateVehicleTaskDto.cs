using System;

namespace InventoryTrackingAutomation.Dtos.Tasks;

/// <summary>
/// Arac-gorev atamasi olusturma request DTO'su.
/// </summary>
//işlevi: CreateVehicleTask verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class CreateVehicleTaskDto
{
    public Guid VehicleId { get; set; }       // Arac Id'si.
    public Guid InventoryTaskId { get; set; } // Gorev Id'si.
    public DateTime AssignedAt { get; set; }  // Atama zamani.
    public DateTime? ReleasedAt { get; set; } // Birakma zamani.
    public bool IsActive { get; set; }        // Aktiflik bilgisi.
}
