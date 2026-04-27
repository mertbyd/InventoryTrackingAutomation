using System;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Movements;

/// <summary>
/// Lokasyonlar arası malzeme transfer taleplerini temsil eden entity.
/// </summary>
public class MovementRequest : FullAuditedAggregateRoot<Guid>
{
    public string RequestNumber { get; set; }               // Talebin benzersiz numarası. Örnek: "MR-2024-00123"
    public Guid RequestedByWorkerId { get; set; }           // Talebi iş sürecinde oluşturan çalışanın kimliği. Örnek: Worker Id'si
    public Guid SourceSiteId { get; set; }                  // Malzemenin çıkacağı kaynak lokasyon. Örnek: Site Id'si (depo)
    public Guid TargetSiteId { get; set; }                  // Malzemenin gideceği hedef lokasyon. Örnek: Site Id'si (saha)
    public Guid? RequestedVehicleId { get; set; }            // Talep edilen sevkiyat aracı. Örnek: Vehicle Id'si
    public MovementStatusEnum Status { get; set; }          // Talebin anlık durumu. Örnek: MovementStatusEnum.Pending
    public MovementPriorityEnum Priority { get; set; }      // Talebin öncelik seviyesi. Örnek: MovementPriorityEnum.Normal
    public string RequestNote { get; set; }                 // Talep gerekçesi ve açıklaması. Örnek: "Saha üretimi için acil malzeme ihtiyacı"
    public DateTime PlannedDate { get; set; }               // Malzemenin sahada olması gereken tarih. Örnek: DateTime.UtcNow.AddDays(3)
    public Guid? ShipmentId { get; set; }                   // Talebin bağlı olduğu sevkiyat kimliği. Örnek: Shipment Id'si
    public string? CancellationNote { get; set; }           // İptal gerekçesi. Örnek: "İhtiyaç planı değişti"
    public Guid? WorkflowInstanceId { get; set; }           // Bağlı olduğu iş akışı süreci Id'si.

    protected MovementRequest() { }
    public MovementRequest(Guid id) : base(id) { }
}
