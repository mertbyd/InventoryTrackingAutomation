using System;

namespace InventoryTrackingAutomation.Models.Tasks;

public class CreateTaskLineModel
{
    public Guid TaskId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
