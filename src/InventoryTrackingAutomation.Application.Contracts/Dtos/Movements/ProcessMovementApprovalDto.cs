using System.ComponentModel.DataAnnotations;

namespace InventoryTrackingAutomation.Dtos.Movements;

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
    /// Note alanı.
    /// </summary>
    public string Note { get; set; }
}
