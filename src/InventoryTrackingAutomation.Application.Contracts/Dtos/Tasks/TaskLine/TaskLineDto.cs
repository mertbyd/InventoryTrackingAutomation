using System;
using Volo.Abp.Application.Dtos;
using InventoryTrackingAutomation.Dtos.Masters;

namespace InventoryTrackingAutomation.Dtos.Tasks;

/// <summary>
/// TaskLine verisinin transferi sirasinda tasinacak olan yapiyi tanimlar.
/// </summary>
//işlevi: TaskLine verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class TaskLineDto : EntityDto<Guid>
{
    /// <summary>Gorev baglami.</summary>
    public Guid TaskId { get; set; }

    /// <summary>Talep edilen toplam miktar.</summary>
    public int Quantity { get; set; }

    /// <summary>Urun (Product navigation'ından nested doldurulur).</summary>
    public ProductRefDto Product { get; set; }
}
