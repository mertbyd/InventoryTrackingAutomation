using System;
using InventoryTrackingAutomation.Enums.Inventory;

namespace InventoryTrackingAutomation.Models.Inventory;

/// <summary>
/// Stok transfer islemi icin model.
/// </summary>
public class StockTransferModel
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public StockLocationTypeEnum SourceLocationType { get; set; }
    public Guid SourceLocationId { get; set; }
    public StockLocationTypeEnum DestinationLocationType { get; set; }
    public Guid DestinationLocationId { get; set; }
    public InventoryTransactionTypeEnum TransactionType { get; set; }
    public Guid? RelatedMovementRequestId { get; set; }
    public Guid? RelatedTaskId { get; set; }
    public Guid? PerformedByUserId { get; set; }
    public string? Note { get; set; }
}
