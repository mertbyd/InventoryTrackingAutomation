namespace InventoryTrackingAutomation.Enums;

/// <summary>
/// Envanter gorevinin yasam dongusu durumlari.
/// </summary>
public enum InventoryTaskStatusEnum
{
    Draft = 1,      // Gorev hazirlik asamasinda.
    InProgress = 2, // Gorev sahada aktif.
    Completed = 3   // Gorev tamamlandi.
}
