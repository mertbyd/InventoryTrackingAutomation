using System;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Shipments;

/// <summary>
/// Sevkiyat güncelleme request DTO'su.
/// </summary>
public class UpdateShipmentDto
{
    public string ShipmentNumber { get; set; }          // Sevkiyat numarası. Örnek: "SHP-2024-00456"
    public Guid VehicleId { get; set; }                 // Araç Id.
    public Guid DriverWorkerId { get; set; }            // Sürücü çalışan Id.
    public ShipmentStatusEnum Status { get; set; }      // Sevkiyat durumu. Örnek: ShipmentStatusEnum.Preparing
    public DateTime? PlannedDepartureTime { get; set; } // Planlanan hareket zamanı.
    public string? Note { get; set; }                   // Teslimat notu.
}
