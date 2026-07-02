using System;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Tasks;

/// <summary>
/// VehicleTaskLine verisinin transferi sirasinda tasinacak olan yapiyi tanimlar.
/// </summary>
//işlevi: VehicleTaskLine verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class VehicleTaskLineDto : EntityDto<Guid>
{
    /// <summary>Arac-gorev atamasi baglami.</summary>
    public Guid VehicleTaskId { get; set; }

    /// <summary>Kaynak gorev kalemi baglami.</summary>
    public Guid TaskLineId { get; set; }

    /// <summary>Bu araca tahsis edilen miktar.</summary>
    public int AllocatedQuantity { get; set; }

    /// <summary>Iade tesliminde depoya saglam giren miktar.</summary>
    public int ReceivedQuantity { get; set; }

    /// <summary>Iade tesliminde hasarli olarak ayrilan miktar.</summary>
    public int DamagedQuantity { get; set; }

    /// <summary>Iade tesliminde kayip olarak isaretlenen miktar.</summary>
    public int LostQuantity { get; set; }

    /// <summary>Gorevde tuketildigi bildirilen miktar.</summary>
    public int ConsumedQuantity { get; set; }

    /// <summary>Satir bazli teslim alma notu.</summary>
    public string? ReceiveNote { get; set; }

    /// <summary>Kaynak gorev kalemi (TaskLine navigation'ından nested; urune buradan ulasilir).</summary>
    public TaskLineDto TaskLine { get; set; }
}
