using System;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Masters;

/// <summary>
/// Sistemdeki araçları temsil eden master data entity'si.
/// </summary>
public class Vehicle : FullAuditedEntity<Guid>
{
    public string PlateNumber { get; set; }           // Araç plaka numarası. Örnek: "34 ABC 123"
    public VehicleTypeEnum VehicleType { get; set; } // Araç tipi. Örnek: VehicleTypeEnum.Van
    public bool IsActive { get; set; }               // Araç aktif mi. Örnek: true

    protected Vehicle() { }
    public Vehicle(Guid id) : base(id) { }
}
