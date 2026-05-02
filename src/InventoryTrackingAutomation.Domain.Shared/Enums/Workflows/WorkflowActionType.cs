namespace InventoryTrackingAutomation.Enums.Workflows;

/// <summary>
/// Is akisi adimi uzerinde alinan aksiyon tipini temsil eden enum.
/// </summary>
public enum WorkflowActionType : byte
{
    /// <summary>
    /// Onay bekliyor.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Onaylandi.
    /// </summary>
    Approved = 1,

    /// <summary>
    /// Reddedildi.
    /// </summary>
    Rejected = 2
}
