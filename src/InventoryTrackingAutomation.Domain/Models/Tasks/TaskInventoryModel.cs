using System;

namespace InventoryTrackingAutomation.Models.Tasks;

/// <summary>
/// Bir gorev kapsamindaki arac stok kaydini temsil eder.
/// </summary>
public class TaskInventoryModel
{
    public Guid InventoryTaskId { get; set; } // Gorev Id'si.
    public Guid VehicleTaskId { get; set; }   // Gorev-arac atama Id'si.
    public Guid VehicleId { get; set; }       // Arac Id'si.
    public Guid ProductId { get; set; }       // Arac uzerindeki urun Id'si.
    public int Quantity { get; set; }         // Gorev kapsamindaki miktar.
    public int ReservedQuantity { get; set; } // Rezerve miktar.
}
