using System;

namespace InventoryTrackingAutomation.Dtos.Tasks;

/// <summary>
/// Gorev kapsamindaki arac envanterini donduren DTO.
/// </summary>
//işlevi: TaskInventory verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class TaskInventoryDto
{
    public Guid InventoryTaskId { get; set; } // Gorev Id'si.
    public Guid VehicleTaskId { get; set; }   // Gorev-arac atama Id'si.
    public Guid VehicleId { get; set; }       // Arac Id'si.
    public Guid ProductId { get; set; }       // Urun Id'si.
    public int Quantity { get; set; }         // Fiziksel miktar.
    public int ReservedQuantity { get; set; } // Rezerve miktar.
}
