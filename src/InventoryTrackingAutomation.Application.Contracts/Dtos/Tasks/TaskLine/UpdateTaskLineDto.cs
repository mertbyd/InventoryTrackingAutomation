using System;

namespace InventoryTrackingAutomation.Dtos.Tasks;

/// <summary>
/// TaskLine guncelleme verisini tanimlar.
/// </summary>
//işlevi: UpdateTaskLine verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class UpdateTaskLineDto
{
    /// <summary>Urun baglami.</summary>
    public Guid ProductId { get; set; }     // Urun baglami.
    /// <summary>Yeni miktar.</summary>
    public int Quantity { get; set; }       // Yeni miktar.
}
