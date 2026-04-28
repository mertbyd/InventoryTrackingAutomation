using System;
using System.Collections.Generic;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Models.Movements;

/// <summary>
/// Hareket talebi + satırları için domain modeli — AppService'ten Manager'a taşınan veri taşıyıcı.
/// </summary>
public class CreateMovementRequestWithLinesModel
{
    public string RequestNumber { get; set; }
    public Guid RequestedByWorkerId { get; set; }      // Server-side: AppService CurrentUser'dan çözer.
    public Guid SourceWarehouseId { get; set; }
    public Guid? TargetWarehouseId { get; set; }
    public Guid? RequestedVehicleId { get; set; }
    public Guid? AssignedTaskId { get; set; }
    public MovementPriorityEnum Priority { get; set; }
    public string RequestNote { get; set; }
    public DateTime PlannedDate { get; set; }
    public List<CreateMovementRequestLineItemModel> Lines { get; set; } = new();
}

/// <summary>
/// With-lines domain modelindeki tek satır.
/// </summary>
public class CreateMovementRequestLineItemModel
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
