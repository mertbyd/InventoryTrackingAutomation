using System;
using Volo.Abp.Application.Dtos;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Movements;

/// <summary>
/// Hareket talebi response DTO'su.
/// </summary>
//işlevi: MovementRequest verisini istemciye (frontend) taşır.
//sistemdeki görevii: Veri tabanı modelini gizleyerek sadece istemcinin ihtiyacı olan talep bilgilerini sunar.
public class MovementRequestDto : FullAuditedEntityDto<Guid>
{
    public string RequestNumber { get; set; }             // Talep numarasi. Ornek: "MR-2024-00123"
    public Guid RequestedByWorkerId { get; set; }         // Talebi olusturan calisan Id.
    public Guid SourceWarehouseId { get; set; }                // Kaynak lokasyon Id.
    public Guid? TargetWarehouseId { get; set; }                // Hedef lokasyon Id. (Opsiyonel)
    public Guid? RequestedVehicleId { get; set; }          // Talep edilen arac Id.
    public Guid? AssignedTaskId { get; set; }             // Bagli gorev Id.
    public MovementStatusEnum Status { get; set; }        // Talep durumu. Ornek: MovementStatusEnum.Pending
    public MovementPriorityEnum Priority { get; set; }    // Oncelik. Ornek: MovementPriorityEnum.Normal
    public string RequestNote { get; set; }               // Talep gerekcesi.
    public DateTime PlannedDate { get; set; }             // Planlanan teslim tarihi.
    public string? CancellationNote { get; set; }         // Iptal gerekcesi.
    public Guid? WorkflowInstanceId { get; set; }         // Bagli is akisi Id'si.
}
