using System;

namespace InventoryTrackingAutomation.Dtos.Inventory;

//işlevi: VehicleInventory verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class VehicleInventoryDto
{
    /// <summary>
    /// Arac Id&apos;si.
    /// </summary>
    public Guid VehicleId { get; set; }        // Arac Id'si.
    /// <summary>
    /// Urun Id&apos;si.
    /// </summary>
    public Guid ProductId { get; set; }        // Urun Id'si.
    /// <summary>
    /// Aktif gorev-arac atama Id&apos;si.
    /// </summary>
    public Guid? VehicleTaskId { get; set; }   // Aktif gorev-arac atama Id'si.
    /// <summary>
    /// Aktif operasyon isi Id&apos;si.
    /// </summary>
    public Guid? TaskId { get; set; } // Aktif operasyon isi Id'si.
    /// <summary>
    /// Fiziksel miktar.
    /// </summary>
    public int Quantity { get; set; }          // Fiziksel miktar.
    /// <summary>
    /// Rezerve miktar.
    /// </summary>
    public int ReservedQuantity { get; set; }  // Rezerve miktar.

    public string VehicleName { get; set; }
    public string ProductName { get; set; }
    public string VehicleTaskName { get; set; }
    public string TaskName { get; set; }
}

