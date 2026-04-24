using System;
using Volo.Abp.Application.Dtos;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Shipments;

/// <summary>
/// Sevkiyat response DTO'su — GetAll ve GetById operasyonlarında döner.
/// </summary>
public class ShipmentDto : FullAuditedEntityDto<Guid>
{
    public string ShipmentNumber { get; set; }         // Sevkiyat numarası. Örnek: "SHP-2024-00456"
    public Guid VehicleId { get; set; }                // Araç Id.
    public Guid DriverWorkerId { get; set; }           // Sürücü çalışan Id.
    public ShipmentStatusEnum Status { get; set; }     // Sevkiyat durumu. Örnek: ShipmentStatusEnum.Preparing
    public DateTime? PlannedDepartureTime { get; set; }// Planlanan hareket zamanı.
    public DateTime? DepartureTime { get; set; }       // Gerçekleşen hareket zamanı.
    public DateTime? DeliveryTime { get; set; }        // Teslim zamanı.
    public string? Note { get; set; }                  // Teslimat notu.
}
