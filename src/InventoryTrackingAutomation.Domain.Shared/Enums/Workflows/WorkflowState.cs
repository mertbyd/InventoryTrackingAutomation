namespace InventoryTrackingAutomation.Enums.Workflows;

/// <summary>
/// Is akisi durumunu temsil eden enum.
/// </summary>
public enum WorkflowState : byte
{
    /// <summary>
    /// Is akisi devam ediyor.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Is akisi basariyla tamamlandi.
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Is akisi reddedildi.
    /// </summary>
    Rejected = 3
}
