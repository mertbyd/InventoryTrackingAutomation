using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation.Entities.Masters;

/// <summary>
/// Sahada stok tasiyabilen araci temsil eden master aggregate.
/// </summary>
public class Vehicle : FullAuditedEntity<Guid>
{
    public string PlateNumber { get; set; } = default!; // Aracin operasyonel plaka bilgisini tasir.
    public VehicleTypeEnum VehicleType { get; set; } // Aracin saha operasyonundaki tipini belirler.
    public bool IsActive { get; set; } // Aracin gorevlere atanabilir olup olmadigini belirler.

    protected Vehicle() { }
    public Vehicle(Guid id) : base(id) { }
}
