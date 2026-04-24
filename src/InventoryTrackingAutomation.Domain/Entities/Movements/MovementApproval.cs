using System;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Movements;

/// <summary>
/// Hareket talebinin çok aşamalı onay sürecindeki her onaycının kararını tutan entity.
/// </summary>
public class MovementApproval : FullAuditedAggregateRoot<Guid>
{
    public Guid MovementRequestId { get; set; }         // Bağlı olduğu talep kimliği. Örnek: MovementRequest Id'si
    public Guid ApproverWorkerId { get; set; }          // Bu adımda karar veren onaycı çalışanın kimliği. Örnek: Worker Id'si
    public int StepOrder { get; set; }                  // Onay sırası (1'den başlar). Örnek: 1 = 1. Onaycı, 2 = 2. Onaycı
    public ApprovalStatusEnum Status { get; set; }      // Bu adımın durumu. Örnek: ApprovalStatusEnum.Pending
    public DateTime? DecidedAt { get; set; }            // Kararın verildiği tarih ve saat. Örnek: DateTime.UtcNow
    public string? Note { get; set; }                   // Onaycının yorumu veya red gerekçesi. Örnek: "Stok yetersiz, 2 hafta sonra tekrar değerlendirilecek"

    protected MovementApproval() { }
    public MovementApproval(Guid id) : base(id) { }
}
