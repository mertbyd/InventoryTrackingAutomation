using System;

namespace InventoryTrackingAutomation.Dtos.Tasks;

/// <summary>
/// Iade teslim alma sirasinda arac kaleminin uzlasma verisini tasir.
/// </summary>
//işlevi: ReceiveVehicleTaskLine verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class ReceiveVehicleTaskLineDto
{
    /// <summary>Iade tesliminde depoya saglam giren miktar.</summary>
    public int ReceivedQuantity { get; set; }   // Iade tesliminde depoya saglam giren miktar.
    /// <summary>Iade tesliminde hasarli olarak ayrilan miktar.</summary>
    public int DamagedQuantity { get; set; }    // Iade tesliminde hasarli olarak ayrilan miktar.
    /// <summary>Iade tesliminde kayip olarak isaretlenen miktar.</summary>
    public int LostQuantity { get; set; }       // Iade tesliminde kayip olarak isaretlenen miktar.
    /// <summary>Gorevde tuketildigi bildirilen miktar.</summary>
    public int ConsumedQuantity { get; set; }   // Gorevde tuketildigi bildirilen miktar.
    /// <summary>Satir bazli teslim alma notu.</summary>
    public string? ReceiveNote { get; set; }    // Satir bazli teslim alma notu.
}
