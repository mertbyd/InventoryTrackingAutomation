using System.ComponentModel.DataAnnotations;

namespace InventoryTrackingAutomation.Dtos.Movements;

/// <summary>
/// Hareket talebini işleme (onay/red) isteği.
/// </summary>
//işlevi: ProcessMovementApproval verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class ProcessMovementApprovalDto
{
    /// <summary>
    /// İşlemin onay olup olmadığını belirtir.
    /// true = Onayla, false = Reddet
    /// </summary>
    [Required(ErrorMessage = "Onay durumu (IsApproved) belirtilmelidir.")]
    public bool IsApproved { get; set; }

    /// <summary>
    /// Onaylayan kişinin yorumu veya ret nedeni. Red durumunda zorunludur.
    /// </summary>
    public string Note { get; set; }
}
