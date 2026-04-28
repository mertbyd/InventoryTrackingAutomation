using System;
using System.Collections.Generic;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Movements;

/// <summary>
/// Hareket talebi + satırlarını tek atomik istekle oluşturmak için DTO.
/// RequestedByWorkerId, WorkflowInstanceId, Status server-side çözümlenir; client'tan alınmaz.
/// </summary>
//işlevi: CreateMovementRequestWithLines verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class CreateMovementRequestWithLinesDto
{
    public string RequestNumber { get; set; }          // Talep numarası. Örnek: "MR-2024-00123"
    public Guid SourceWarehouseId { get; set; }             // Kaynak lokasyon Id.
    public Guid TargetWarehouseId { get; set; }             // Hedef lokasyon Id.
    public Guid RequestedVehicleId { get; set; }        // Talep edilen sevkiyat aracı Id.
    public MovementPriorityEnum Priority { get; set; } // Öncelik. Örnek: MovementPriorityEnum.Normal
    public string RequestNote { get; set; }            // Talep gerekçesi.
    public DateTime PlannedDate { get; set; }          // Planlanan teslim tarihi.
    public List<CreateMovementRequestLineItemDto> Lines { get; set; } = new(); // Talep satırları (en az 1 zorunlu).
}

/// <summary>
/// With-lines isteğindeki tek satır — MovementRequestId server-side set edileceği için burada YOK.
/// </summary>
//işlevi: CreateMovementRequestWithLines verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class CreateMovementRequestLineItemDto
{
    public Guid ProductId { get; set; } // Ürün Id.
    public int Quantity { get; set; }   // Talep edilen miktar. Örnek: 20
}
