using System;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Application.Dtos;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using System;

namespace InventoryTrackingAutomation.Dtos.Movements;

//işlevi: MovementRequest verisini istemciye (frontend) taşır.
//sistemdeki görevii: Veri tabanı modelini gizleyerek sadece istemcinin ihtiyacı olan talep bilgilerini sunar.
//işlevi: MovementRequest verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class MovementRequestDto : EntityDto<Guid>
{
    /// <summary>
    /// Talep numarasi. Ornek: &quot;MR-2024-00123&quot;
    /// </summary>
    public string RequestNumber { get; set; }             // Talep numarasi. Ornek: "MR-2024-00123"
    /// <summary>
    /// Talebi olusturan calisan Id.
    /// </summary>
    public Guid RequestedByWorkerId { get; set; }         // Talebi olusturan calisan Id.
    /// <summary>
    /// Bagli operasyon isi Id.
    /// </summary>
    public Guid TaskId { get; set; }                      // Bagli operasyon isi Id.
    /// <summary>
    /// Arac-operasyon atamasi Id.
    /// </summary>
    public Guid VehicleTaskId { get; set; }               // Arac-operasyon atamasi Id.
    /// <summary>
    /// Iade hareketlerinde ana talep Id.
    /// </summary>
    public Guid? ParentMovementRequestId { get; set; }    // Iade hareketlerinde ana talep Id.
    /// <summary>
    /// Talep durumu. Ornek: MovementStatusEnum.Pending
    /// </summary>
    public MovementStatusEnum Status { get; set; }        // Talep durumu. Ornek: MovementStatusEnum.Pending
    /// <summary>
    /// Oncelik. Ornek: MovementPriorityEnum.Normal
    /// </summary>
    public MovementPriorityEnum Priority { get; set; }    // Oncelik. Ornek: MovementPriorityEnum.Normal
    /// <summary>
    /// Talep gerekcesi.
    /// </summary>
    public string RequestNote { get; set; }               // Talep gerekcesi.
    /// <summary>
    /// Planlanan teslim tarihi.
    /// </summary>
    public DateTime PlannedDate { get; set; }             // Planlanan teslim tarihi.
    /// <summary>
    /// Iptal gerekcesi.
    /// </summary>
    public string? CancellationNote { get; set; }         // Iptal gerekcesi.
    /// <summary>
    /// Bagli is akisi Id&apos;si.
    /// </summary>
    public Guid? WorkflowInstanceId { get; set; }         // Bagli is akisi Id'si.
}
