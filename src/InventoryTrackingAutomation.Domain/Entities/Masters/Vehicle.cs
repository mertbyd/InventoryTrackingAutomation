using System;
using InventoryTrackingAutomation.Entities;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Masters;

/// <summary>
/// Sahada stok tasiyabilen araci temsil eden master aggregate.
/// </summary>
// islevi: Aracin plaka, tip ve aktiflik bilgilerini tasir.
// sistemdeki gorevi: VehicleTask atamalarinda stok tasima kapasitesi olan operasyonel varligi temsil eder.
public class Vehicle : AuditedEntity<Guid>, IPassivable
{
    public string PlateNumber { get; set; } = default!; // Aracin operasyonel plaka bilgisini tasir.
    public Guid VehicleTypeId { get; set; } // Aracin saha operasyonundaki tipini (Lookup FK) belirler.
    public bool IsActive { get; set; } // Aracin gorevlere atanabilir olup olmadigini belirler.

    protected Vehicle() { }
    public Vehicle(Guid id) : base(id) { }
}
