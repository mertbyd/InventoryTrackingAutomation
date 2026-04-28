using System;
using System.Collections.Generic;

namespace InventoryTrackingAutomation.Dtos.Movements;

/// <summary>
/// Hareket talebinin teslim alinmasi icin kullanilan DTO.
/// </summary>
public class ReceiveMovementRequestDto
{
    public string? ReceiveNote { get; set; } // Teslim alma notu.
    public List<ReceiveMovementRequestLineDto> Lines { get; set; } = new(); // Iade tesliminde satir bazli kontrol sonucu.
}

/// <summary>
/// Gorev iade tesliminde tek urun satirinin kontrol sonucunu tasir.
/// </summary>
public class ReceiveMovementRequestLineDto
{
    public Guid ProductId { get; set; } // Iade edilen urun.
    public int ReceivedQuantity { get; set; } // Depoya saglam alinacak miktar.
    public int DamagedQuantity { get; set; } // Hasarli/kirik olarak ayrilan miktar.
    public int LostQuantity { get; set; } // Kayip olarak isaretlenen miktar.
    public int ConsumedQuantity { get; set; } // Gorevde tuketildigi kabul edilen miktar.
    public string? Note { get; set; } // Satir bazli kontrol notu.
}
