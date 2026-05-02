using System;

namespace InventoryTrackingAutomation.Dtos.Tasks;

/// <summary>
/// Yeni VehicleTaskLine olusturma verisini tanimlar.
/// </summary>
//işlevi: CreateVehicleTaskLine verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class CreateVehicleTaskLineDto
{
    /// <summary>Araca tahsis edilecek gorev kalemi baglami.</summary>
    public Guid TaskLineId { get; set; }        // Urun bilgisi TaskLine uzerinden cozulur.
    /// <summary>Bu araca tahsis edilen miktar.</summary>
    public int AllocatedQuantity { get; set; }  // Bu araca tahsis edilen miktar.
}
