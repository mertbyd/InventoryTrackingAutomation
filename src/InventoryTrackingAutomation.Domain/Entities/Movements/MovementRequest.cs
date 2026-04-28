using System;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Movements;

/// <summary>
/// Lokasyonlar arasi malzeme transfer taleplerini temsil eden entity.
/// </summary>
public class MovementRequest : FullAuditedAggregateRoot<Guid>
{
    public string RequestNumber { get; set; }               // Talebin benzersiz numarasi. Ornek: "MR-2024-00123"
    public Guid RequestedByWorkerId { get; set; }           // Talebi is surecinde olusturan calisanin kimligi.
    public Guid SourceSiteId { get; set; }                  // Malzemenin cikacagi kaynak lokasyon. Ornek: depo Site Id'si
    public Guid TargetSiteId { get; set; }                  // Malzemenin gidecegi hedef lokasyon. Ornek: hedef Site Id'si
    public Guid? RequestedVehicleId { get; set; }            // Talep edilen arac. Ornek: Vehicle Id'si
    public MovementStatusEnum Status { get; set; }          // Talebin anlik durumu. Ornek: MovementStatusEnum.Pending
    public MovementPriorityEnum Priority { get; set; }      // Talebin oncelik seviyesi. Ornek: MovementPriorityEnum.Normal
    public string RequestNote { get; set; }                 // Talep gerekcesi ve aciklamasi.
    public DateTime PlannedDate { get; set; }               // Malzemenin hedef lokasyonda olmasi gereken tarih.
    public string? CancellationNote { get; set; }           // Iptal gerekcesi. Ornek: "Ihtiyac plani degisti"
    public Guid? WorkflowInstanceId { get; set; }           // Bagli oldugu is akisi sureci Id'si.

    protected MovementRequest() { }
    public MovementRequest(Guid id) : base(id) { }
}
