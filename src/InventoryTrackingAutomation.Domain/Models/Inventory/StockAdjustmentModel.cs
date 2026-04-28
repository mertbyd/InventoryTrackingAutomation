using System;
using InventoryTrackingAutomation.Enums.Inventory;

namespace InventoryTrackingAutomation.Models.Inventory;

/// <summary>
/// Stok sayim/kayip/hasar/tuketim duzeltmesi icin domain modeli.
/// </summary>
public class StockAdjustmentModel
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public StockLocationTypeEnum SourceLocationType { get; set; }
    public Guid SourceLocationId { get; set; }
    public Guid? RelatedMovementRequestId { get; set; }
    public Guid? RelatedTaskId { get; set; }
    public Guid? PerformedByUserId { get; set; }
    public string? Note { get; set; }
}
