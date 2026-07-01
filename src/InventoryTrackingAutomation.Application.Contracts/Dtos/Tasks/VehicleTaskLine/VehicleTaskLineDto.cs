using System;
using Volo.Abp.Application.Dtos;
using InventoryTrackingAutomation.Dtos.Common;

namespace InventoryTrackingAutomation.Dtos.Tasks;

/// <summary>
/// VehicleTaskLine verisinin transferi sirasinda tasinacak olan yapiyi tanimlar.
/// </summary>
//işlevi: VehicleTaskLine verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class VehicleTaskLineDto : EnrichedEntityDto<Guid>
{
    /// <summary>Arac-gorev atamasi baglami.</summary>
    public Guid VehicleTaskId { get; set; }     // Arac-gorev atamasi baglami.
    /// <summary>Kaynak gorev kalemi baglami.</summary>
    public Guid TaskLineId { get; set; }        // Kaynak gorev kalemi baglami.
    /// <summary>TaskLine uzerinden turetilen urun baglami.</summary>
    public Guid ProductId { get; set; }         // Entity kolonu degildir; TaskLine.ProductId uzerinden doldurulur.
    /// <summary>Bu araca tahsis edilen miktar.</summary>
    public int AllocatedQuantity { get; set; }  // Bu araca tahsis edilen miktar.
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
