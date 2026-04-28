using System;

namespace InventoryTrackingAutomation.Dtos.Stock;

/// <summary>
/// Arac uzerindeki urun stok bilgisini donduren DTO.
/// </summary>
public class VehicleInventoryDto
{
    public Guid VehicleId { get; set; }        // Arac Id'si.
    public Guid ProductId { get; set; }        // Urun Id'si.
    public Guid? VehicleTaskId { get; set; }   // Aktif gorev-arac atama Id'si.
    public Guid? InventoryTaskId { get; set; } // Aktif gorev Id'si.
    public int Quantity { get; set; }          // Fiziksel miktar.
    public int ReservedQuantity { get; set; }  // Rezerve miktar.
}
