using System;
using System.Collections.Generic;

namespace InventoryTrackingAutomation.Dtos.Movements;

public class ReceiveMovementRequestDto
{
    /// <summary>
    /// Teslim alma notu.
    /// </summary>
    public string? ReceiveNote { get; set; } // Teslim alma notu.
    public List<ReceiveMovementRequestVehicleTaskLineDto> Lines { get; set; } = new(); // Iade tesliminde satir bazli kontrol sonucu.
}

public class ReceiveMovementRequestVehicleTaskLineDto
{
    /// <summary>
    /// Uzlastirilacak arac-gorev kalem Id'si.
    /// </summary>
    public Guid VehicleTaskLineId { get; set; } // Uzlastirilacak arac-gorev kalem Id'si.
    /// <summary>
    /// Depoya saglam alinacak miktar.
    /// </summary>
    public int ReceivedQuantity { get; set; } // Depoya saglam alinacak miktar.
    /// <summary>
    /// Hasarli/kirik olarak ayrilan miktar.
    /// </summary>
    public int DamagedQuantity { get; set; } // Hasarli/kirik olarak ayrilan miktar.
    /// <summary>
    /// Kayip olarak isaretlenen miktar.
    /// </summary>
    public int LostQuantity { get; set; } // Kayip olarak isaretlenen miktar.
    /// <summary>
    /// Gorevde tuketildigi kabul edilen miktar.
    /// </summary>
    public int ConsumedQuantity { get; set; } // Gorevde tuketildigi kabul edilen miktar.
    /// <summary>
    /// Satir bazli kontrol notu.
    /// </summary>
    public string? Note { get; set; } // Satir bazli kontrol notu.

    public string VehicleTaskLineName { get; set; }
}

