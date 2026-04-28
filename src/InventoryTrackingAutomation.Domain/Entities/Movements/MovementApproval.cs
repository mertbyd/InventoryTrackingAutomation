using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation.Entities.Movements;

/// <summary>
/// Hareket talebinin onay surecindeki karar izini temsil eden aggregate.
/// </summary>
public class MovementApproval : FullAuditedEntity<Guid>
{
    public Guid MovementRequestId { get; set; } // Kararin bagli oldugu talep baglamini tasir.
    public Guid ApproverWorkerId { get; set; } // Karari verecek veya veren onayci calisan baglamini tasir.
    public int StepOrder { get; set; } // Onay zincirindeki sira bilgisini tasir.
    public ApprovalStatusEnum Status { get; set; } // Onay adiminin karar durumunu belirler.
    public DateTime? DecidedAt { get; set; } // Kararin verildigi zamani tasir.
    public string? Note { get; set; } // Onaycinin operasyonel notunu veya red gerekcesini tasir.

    protected MovementApproval() { }
    public MovementApproval(Guid id) : base(id) { }
}
