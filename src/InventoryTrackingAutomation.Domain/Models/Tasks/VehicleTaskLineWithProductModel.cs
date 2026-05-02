using System;
using InventoryTrackingAutomation.Entities.Tasks;

namespace InventoryTrackingAutomation.Models.Tasks;

/// <summary>
/// Arac-gorev kalemi ile urun bilgisini bir arada tasiyan model.
/// </summary>
public class VehicleTaskLineWithProductModel
{
    public VehicleTaskLine Line { get; set; }
    public Guid ProductId { get; set; }

    public VehicleTaskLineWithProductModel(VehicleTaskLine line, Guid productId)
    {
        Line = line;
        ProductId = productId;
    }
}
