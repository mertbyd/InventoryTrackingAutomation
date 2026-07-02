using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation.Entities.Movements;

/// <summary>
/// Depo, arac ve gorev baglaminda malzeme transfer talebini temsil eden aggregate.
/// </summary>
//işlevi: Stok hareketi için gereken izin/onay sürecini başlatır ve hedef rotayı (araç veya depo) tutar.
//sistemdeki görevii: Onaylanana kadar stoklara dokunmayan, sadece "Transfer Niyeti"ni barındıran temel bilet tablosudur.
public class MovementRequest : FullAuditedEntity<Guid>
{
    public string RequestNumber { get; set; } = default!; // Talebin kurumsal takip numarasini tasir.
    public Guid RequestedByWorkerId { get; set; } // Talebi olusturan calisan baglamini tasir.
    public Guid VehicleTaskId { get; set; } // Talebi tasiyan arac-operasyon atamasi baglamini tasir; TaskId bu iliskiden turetilir.
    public Guid? ParentMovementRequestId { get; set; } // Iade hareketlerinde ana hareket talebi baglamini tasir.
    public MovementStatusEnum Status { get; set; } // Talebin operasyonel durumunu belirler.
    public MovementPriorityEnum Priority { get; set; } // Talebin oncelik seviyesini belirler.
    public string RequestNote { get; set; } = default!; // Talep gerekcesi ve operasyon notunu tasir.
    public DateTime PlannedDate { get; set; } // Malzemenin hedefte beklenen zamanini tasir.
    public string? CancellationNote { get; set; } // Iptal durumunda gerekce baglamini tasir.
    public Guid? WorkflowInstanceId { get; set; } // Talebin bagli oldugu workflow sureci baglamini tasir.

    public virtual VehicleTask VehicleTask { get; protected set; }
    public virtual Worker RequestedByWorker { get; protected set; }
    public virtual MovementRequest? ParentMovementRequest { get; protected set; }

    protected MovementRequest() { }

    //işlevi: Aggregate root'u verilen ID ile ilklendirir.
    //sistemdeki görevii: Yeni talep kayitlarinin Entity Framework tarafindan ID bazli olusumunu saglar.
    public MovementRequest(Guid id) : base(id) { }
}

