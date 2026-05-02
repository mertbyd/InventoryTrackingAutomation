using System;
using System.Collections.Generic;

namespace InventoryTrackingAutomation.Models.Movements;

/// <summary>
/// Hareket talebi teslim alma domain modeli.
/// </summary>
public class ReceiveMovementRequestModel
{
    public string? ReceiveNote { get; set; }
    public List<ReceiveMovementRequestVehicleTaskLineModel> Lines { get; set; } = new();
}

/// <summary>
/// Gorev iade tesliminde satir bazli kontrol sonucunu tasiyan domain modeli.
/// </summary>
public class ReceiveMovementRequestVehicleTaskLineModel
{
    public Guid VehicleTaskLineId { get; set; } // Uzlastirilacak arac-gorev kalem Id'si.
    public int ReceivedQuantity { get; set; }
    public int DamagedQuantity { get; set; }
    public int LostQuantity { get; set; }
    public int ConsumedQuantity { get; set; }
    public string? Note { get; set; }
}
