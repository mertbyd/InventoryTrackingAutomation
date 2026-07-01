using System;
using Volo.Abp.Application.Dtos;
using InventoryTrackingAutomation.Dtos.Common;

namespace InventoryTrackingAutomation.Dtos.Tasks;

/// <summary>
/// TaskLine verisinin transferi sirasinda tasinacak olan yapiyi tanimlar.
/// </summary>
//işlevi: TaskLine verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class TaskLineDto : EnrichedEntityDto<Guid>
{
    /// <summary>Gorev baglami.</summary>
    public Guid TaskId { get; set; }        // Gorev baglami.
    /// <summary>Urun baglami.</summary>
    public Guid ProductId { get; set; }     // Urun baglami.
    /// <summary>Talep edilen toplam miktar.</summary>
    public int Quantity { get; set; }       // Talep edilen toplam miktar.
    /// <summary>VehicleTaskLine toplamindan turetilen dagitilmis miktar.</summary>
    public int AllocatedQuantity { get; set; } // Entity kolonu degildir; arac satirlarindan hesaplanir.
    /// <summary>Henuz atanmamis kalan miktar.</summary>
    public int RemainingQuantity => Quantity - AllocatedQuantity; // Hesaplanan kalan.
}
