using System;

namespace InventoryTrackingAutomation.Models.Tasks;

public class CreateVehicleTaskLineModel
{
    public Guid VehicleTaskId { get; set; }
    public Guid TaskLineId { get; set; }
    public int AllocatedQuantity { get; set; }
}
