using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation.Entities.Tasks;

/// <summary>
/// Sahada yurutulen operasyonel envanter gorevini temsil eden aggregate.
/// </summary>
public class InventoryTask : FullAuditedEntity<Guid>
{
    public string Code { get; set; } = default!; // Gorevin kurumsal kodunu tasir.
    public string Name { get; set; } = default!; // Gorevin kullaniciya gorunen adini tasir.
    public string Region { get; set; } = default!; // Gorevin saha bolgesi baglamini tasir.
    public DateTime StartDate { get; set; } // Gorevin planlanan baslangic zamanini tasir.
    public DateTime? EndDate { get; set; } // Gorevin planlanan veya gercek bitis zamanini tasir.
    public TaskStatusEnum Status { get; set; } // Gorevin yasam dongusu durumunu belirler.
    public string? Description { get; set; } // Gorevin operasyonel aciklama baglamini tasir.
    public Guid? ReturnWarehouseId { get; set; } // Gorev bitince stoklarin donecegi depo baglamini tasir.
    public bool IsActive { get; set; } // Gorevin operasyonel olarak aktif kabul edilip edilmedigini belirler.

    protected InventoryTask() { }
    public InventoryTask(Guid id) : base(id) { }
}
