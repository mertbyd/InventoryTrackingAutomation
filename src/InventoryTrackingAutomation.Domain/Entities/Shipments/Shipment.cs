using System;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Shipments;

/// <summary>
/// Araçla gerçekleştirilen fiziksel sevkiyatları temsil eden entity.
/// </summary>
public class Shipment : FullAuditedEntity<Guid>
{
    public string ShipmentNumber { get; set; }         // Sevkiyatın benzersiz numarası. Örnek: "SHP-2024-00456"
    public Guid VehicleId { get; set; }                // Sevkiyatta kullanılan araç kimliği. Örnek: Vehicle Id'si
    public Guid DriverWorkerId { get; set; }           // Sürücü çalışanın kimliği. Örnek: Worker Id'si
    public ShipmentStatusEnum Status { get; set; }     // Sevkiyatın anlık durumu. Örnek: ShipmentStatusEnum.Preparing
    public DateTime? PlannedDepartureTime { get; set; } // Planlanan hareket tarihi ve saati. Örnek: DateTime.UtcNow.AddHours(2)
    public DateTime? DepartureTime { get; set; }        // Gerçekleşen araç hareket tarihi ve saati. Örnek: DateTime.UtcNow
    public DateTime? DeliveryTime { get; set; }         // Teslim tarihi ve saati. Örnek: DateTime.UtcNow.AddHours(3)
    public string? Note { get; set; }                   // Teslimat notu veya özel talimat. Örnek: "Güvenlik görevlisine teslim edilecek"

    protected Shipment() { }
    public Shipment(Guid id) : base(id) { }
}
