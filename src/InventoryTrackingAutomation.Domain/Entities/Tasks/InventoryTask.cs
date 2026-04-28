using System;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Tasks;

/// <summary>
/// Sahada yurutulen operasyonel envanter gorevini temsil eder.
/// </summary>
public class InventoryTask : FullAuditedAggregateRoot<Guid>
{
    public string Code { get; set; }                 // Gorevin benzersiz kodu. Ornek: "TASK-IZMIR-001"
    public string Name { get; set; }                 // Gorev adi. Ornek: "Izmir Saha Destek Gorevi"
    public string Region { get; set; }               // Gorevin bolgesi. Ornek: "Izmir"
    public DateTime StartDate { get; set; }          // Gorev baslangic tarihi.
    public DateTime? EndDate { get; set; }           // Gorev bitis tarihi; aktif gorevlerde null olabilir.
    public InventoryTaskStatusEnum Status { get; set; } // Gorevin durum bilgisi.
    public string? Description { get; set; }         // Gorev aciklamasi.
    public bool IsActive { get; set; }               // Gorev aktif kayit mi.

    protected InventoryTask() { }
    public InventoryTask(Guid id) : base(id) { }
}
