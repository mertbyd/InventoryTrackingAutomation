using System;

namespace InventoryTrackingAutomation.Dtos.Tasks;

//işlevi: TaskInventory verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class TaskInventoryDto
{
    /// <summary>
    /// Operasyon isi Id&apos;si.
    /// </summary>
    public Guid TaskId { get; set; } // Operasyon isi Id'si.
    /// <summary>
    /// Gorev-arac atama Id&apos;si.
    /// </summary>
    public Guid VehicleTaskId { get; set; }   // Gorev-arac atama Id'si.
    /// <summary>
    /// Arac Id&apos;si.
    /// </summary>
    public Guid VehicleId { get; set; }       // Arac Id'si.
    /// <summary>
    /// Urun Id&apos;si.
    /// </summary>
    public Guid ProductId { get; set; }       // Urun Id'si.
    /// <summary>
    /// Fiziksel miktar.
    /// </summary>
    public int Quantity { get; set; }         // Fiziksel miktar.
    /// <summary>
    /// Rezerve miktar.
    /// </summary>
    public int ReservedQuantity { get; set; } // Rezerve miktar.
}
