using System;

namespace InventoryTrackingAutomation.Models.Tasks;

public class UpdateTaskLineModel
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
