namespace InventoryTrackingAutomation.Dtos.Movements;

/// <summary>
/// Hareket talebinin fiziksel sevke cikmasi icin kullanilan DTO.
/// </summary>
public class DispatchMovementRequestDto
{
    public string? DispatchNote { get; set; } // Sevk/yukleme notu.
}
