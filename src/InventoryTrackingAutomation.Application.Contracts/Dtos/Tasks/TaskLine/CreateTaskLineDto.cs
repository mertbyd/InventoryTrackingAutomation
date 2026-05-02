using System;

namespace InventoryTrackingAutomation.Dtos.Tasks;

/// <summary>
/// Yeni TaskLine olusturma verisini tanimlar.
/// </summary>
//işlevi: CreateTaskLine verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class CreateTaskLineDto
{
    /// <summary>Urun baglami.</summary>
    public Guid ProductId { get; set; }     // Urun baglami.
    /// <summary>Talep edilen miktar.</summary>
    public int Quantity { get; set; }       // Talep edilen miktar.
}
