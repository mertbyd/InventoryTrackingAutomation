using System;

namespace InventoryTrackingAutomation.Models.Tasks;

/// <summary>
/// Iade teslim alma sirasinda arac kaleminin uzlasma verisini tasir.
/// </summary>
public class ReceiveVehicleTaskLineModel
{
    public Guid VehicleTaskLineId { get; set; }
    public int ReceivedQuantity { get; set; }
    public int DamagedQuantity { get; set; }
    public int LostQuantity { get; set; }
    public int ConsumedQuantity { get; set; }
    public string? ReceiveNote { get; set; }
}
