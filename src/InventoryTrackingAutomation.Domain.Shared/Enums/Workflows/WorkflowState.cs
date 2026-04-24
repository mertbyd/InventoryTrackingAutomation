namespace InventoryTrackingAutomation.Enums.Workflows;

/// <summary>
/// İş akışı durumunu temsil eden enum.
/// </summary>
public enum WorkflowState : byte
{
    /// <summary>
    /// İş akışı devam ediyor.
    /// </summary>
    Active = 1,
    
    /// <summary>
    /// İş akışı başarıyla tamamlandı.
    /// </summary>
    Completed = 2,
    
    /// <summary>
    /// İş akışı reddedildi.
    /// </summary>
    Rejected = 3
}
