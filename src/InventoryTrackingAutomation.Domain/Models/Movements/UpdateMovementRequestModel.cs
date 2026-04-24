using System;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Models.Movements;

/// <summary>
/// Hareket talebi güncelleme domain modeli — Service'ten Manager'a taşınan veri taşıyıcı.
/// </summary>
public class UpdateMovementRequestModel
{
    public string RequestNumber { get; set; }                   // Talep numarası. Örnek: "MR-2024-00123"
    public Guid RequestedByWorkerId { get; set; }               // Talebi oluşturan çalışan Id. Örnek: Worker Id'si
    public Guid SourceSiteId { get; set; }                      // Kaynak lokasyon Id. Örnek: Site Id'si
    public Guid TargetSiteId { get; set; }                      // Hedef lokasyon Id. Örnek: Site Id'si
    public Guid? ShipmentId { get; set; }                       // Bağlı sevkiyat Id. Örnek: Shipment Id'si
    public MovementStatusEnum Status { get; set; }              // Talep durumu. Örnek: MovementStatusEnum.InReview
    public MovementPriorityEnum Priority { get; set; }          // Öncelik. Örnek: MovementPriorityEnum.Normal
    public string RequestNote { get; set; }                     // Talep gerekçesi. Örnek: "Acil malzeme ihtiyacı"
    public DateTime PlannedDate { get; set; }                   // Planlanan teslim tarihi. Örnek: DateTime.UtcNow.AddDays(3)
}
