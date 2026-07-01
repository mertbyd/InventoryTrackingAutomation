using System;

namespace InventoryTrackingAutomation.Models.Movements;

/// <summary>
/// Arac gorev iadesi hesaplamalarinda urun bazli kalan stok miktarini temsil eden model.
/// </summary>
public class TaskVehicleReturnLineModel
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }

    public TaskVehicleReturnLineModel(Guid productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }
}
