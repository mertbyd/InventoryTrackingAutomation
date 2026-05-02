using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Masters;

//işlevi: CreateVehicle verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class CreateVehicleDto
{
    /// <summary>
    /// Plaka numarası. Örnek: &quot;34 ABC 123&quot;
    /// </summary>
    public string PlateNumber { get; set; }           // Plaka numarası. Örnek: "34 ABC 123"
    /// <summary>
    /// Araç tipi. Örnek: VehicleTypeEnum.Van
    /// </summary>
    public VehicleTypeEnum VehicleType { get; set; } // Araç tipi. Örnek: VehicleTypeEnum.Van
    /// <summary>
    /// Aktif mi. Örnek: true
    /// </summary>
    public bool IsActive { get; set; }               // Aktif mi. Örnek: true
}
