using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using System;

namespace InventoryTrackingAutomation.Models.Masters;

/// <summary>
/// Araç oluşturma domain modeli — Service'ten Manager'a taşınan veri taşıyıcı.
/// </summary>
public class CreateVehicleModel
{
    public string PlateNumber { get; set; }           // Plaka numarası. Örnek: "34 ABC 123"
    public Guid VehicleTypeId { get; set; } // Araç tipi. Örnek: (Lookup)
    public bool IsActive { get; set; }               // Aktif mi. Örnek: true
}

