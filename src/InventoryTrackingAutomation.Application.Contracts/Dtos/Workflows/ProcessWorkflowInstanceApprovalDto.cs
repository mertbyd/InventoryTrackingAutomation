namespace InventoryTrackingAutomation.Dtos.Workflows;

//işlevi: ProcessWorkflowInstanceApproval verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class ProcessWorkflowInstanceApprovalDto
{
    /// <summary>
    /// IsApproved alanı.
    /// </summary>
    public bool IsApproved { get; set; }
    /// <summary>
    /// Note alanı.
    /// </summary>
    public string? Note { get; set; }
}
