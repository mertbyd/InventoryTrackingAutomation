using System;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Models.Shipments;

/// <summary>
/// Sevkiyat oluşturma domain modeli — Service'ten Manager'a taşınan veri taşıyıcı.
/// </summary>
public class CreateShipmentModel
{
    public string ShipmentNumber { get; set; }           // Sevkiyat numarası. Örnek: "SHP-2024-00456"
    public Guid VehicleId { get; set; }                  // Araç Id. Örnek: Vehicle Id'si
    public Guid DriverWorkerId { get; set; }             // Sürücü çalışan Id. Örnek: Worker Id'si
    public ShipmentStatusEnum Status { get; set; }       // Sevkiyat durumu. Örnek: ShipmentStatusEnum.Preparing
    public DateTime? PlannedDepartureTime { get; set; }  // Planlanan hareket zamanı. Örnek: DateTime.UtcNow.AddHours(2)
    public DateTime? DepartureTime { get; set; }         // Gerçekleşen hareket zamanı. Örnek: DateTime.UtcNow
    public DateTime? DeliveryTime { get; set; }          // Teslim zamanı. Örnek: DateTime.UtcNow.AddHours(3)
    public string? Note { get; set; }                    // Teslimat notu. Örnek: "Güvenlik görevlisine teslim edilecek"
}
