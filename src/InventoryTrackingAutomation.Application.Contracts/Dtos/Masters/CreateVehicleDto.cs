using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Masters;

/// <summary>
/// Araç oluşturma request DTO'su.
/// </summary>
public class CreateVehicleDto
{
    public string PlateNumber { get; set; }           // Plaka numarası. Örnek: "34 ABC 123"
    public VehicleTypeEnum VehicleType { get; set; } // Araç tipi. Örnek: VehicleTypeEnum.Van
    public bool IsActive { get; set; }               // Aktif mi. Örnek: true
}
