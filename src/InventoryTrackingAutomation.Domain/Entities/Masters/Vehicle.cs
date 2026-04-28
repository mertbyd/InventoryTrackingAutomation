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
public class Vehicle : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public string PlateNumber { get; set; } = default!; // Aracin operasyonel plaka bilgisini tasir.
    public VehicleTypeEnum VehicleType { get; set; } // Aracin saha operasyonundaki tipini belirler.
    public bool IsActive { get; set; } // Aracin gorevlere atanabilir olup olmadigini belirler.
    public Guid? TenantId { get; set; } // Arac verisini kiraci sinirinda tutar.

    protected Vehicle() { }
    public Vehicle(Guid id) : base(id) { }
}
