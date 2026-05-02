namespace InventoryTrackingAutomation.Models.Tasks;

/// <summary>
/// Araç-görev kalemi tahsis miktarı güncelleme modelidir.
/// </summary>
//işlevi: VehicleTaskLine tahsis güncellemesini domain manager'a taşır.
//sistemdeki görevi: TaskLine üzerindeki toplam tahsis ile VehicleTaskLine üzerindeki araç tahsisini tutarlı tutar.
public class UpdateVehicleTaskLineModel
{
    public int AllocatedQuantity { get; set; } // Araç için yeni tahsis miktarı.
}
