using System;

namespace InventoryTrackingAutomation.Models.Inventory;

/// <summary>
/// Bir arac uzerindeki urun stok kaydini temsil eder.
/// </summary>
public class VehicleInventoryModel
{
    public Guid VehicleId { get; set; }        // Stokun bulundugu arac Id'si.
    public Guid ProductId { get; set; }        // Aractaki urun Id'si.
    public Guid? VehicleTaskId { get; set; }   // Arac aktif gorevdeyse VehicleTask Id'si.
    public Guid? InventoryTaskId { get; set; } // Aracin bagli oldugu gorev Id'si.
    public int Quantity { get; set; }          // Aractaki fiziksel miktar.
    public int ReservedQuantity { get; set; }  // Aractaki rezerve miktar.
}
