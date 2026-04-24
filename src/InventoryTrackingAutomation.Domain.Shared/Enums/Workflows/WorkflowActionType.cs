namespace InventoryTrackingAutomation.Enums.Workflows;

/// <summary>
/// İş akışı adımı üzerinde alınan aksiyon tipini temsil eden enum.
/// </summary>
public enum WorkflowActionType : byte
{
    /// <summary>
    /// Onay bekliyor.
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Onaylandı.
    /// </summary>
    Approved = 1,
    
    /// <summary>
    /// Reddedildi.
    /// </summary>
    Rejected = 2
}
