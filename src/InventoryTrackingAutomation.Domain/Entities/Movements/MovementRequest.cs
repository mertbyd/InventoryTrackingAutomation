using System;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace InventoryTrackingAutomation.Entities.Movements;

/// <summary>
/// Depo, arac ve gorev baglaminda malzeme transfer talebini temsil eden aggregate.
/// </summary>
public class MovementRequest : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public string RequestNumber { get; set; } = default!; // Talebin kurumsal takip numarasini tasir.
    public Guid RequestedByWorkerId { get; set; } // Talebi olusturan calisan baglamini tasir.
    public Guid SourceWarehouseId { get; set; } // Malzemenin cikacagi kaynak depo baglamini tasir.
    public Guid TargetWarehouseId { get; set; } // Malzemenin gidecegi hedef depo baglamini tasir.
    public Guid? RequestedVehicleId { get; set; } // Onay sonrasi stok alacak arac baglamini tasir.
    public MovementStatusEnum Status { get; set; } // Talebin operasyonel durumunu belirler.
    public MovementPriorityEnum Priority { get; set; } // Talebin oncelik seviyesini belirler.
    public string RequestNote { get; set; } = default!; // Talep gerekcesi ve operasyon notunu tasir.
    public DateTime PlannedDate { get; set; } // Malzemenin hedefte beklenen zamanini tasir.
    public string? CancellationNote { get; set; } // Iptal durumunda gerekce baglamini tasir.
    public Guid? WorkflowInstanceId { get; set; } // Talebin bagli oldugu workflow sureci baglamini tasir.
    public Guid? TenantId { get; set; } // Talep verisini kiraci sinirinda tutar.

    protected MovementRequest() { }
    public MovementRequest(Guid id) : base(id) { }
}
