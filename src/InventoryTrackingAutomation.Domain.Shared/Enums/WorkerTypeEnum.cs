namespace InventoryTrackingAutomation.Enums;

/// <summary>
/// Sistemdeki çalışan tiplerini tanımlayan enum.
/// </summary>
public enum WorkerTypeEnum
{
    WhiteCollar = 1,    // Ofis/büro çalışanları için kullanılır. Örnek: Muhasebe, İK personeli
    BlueCollar = 2,     // Saha/üretim çalışanları için kullanılır. Örnek: Operatör, teknisyen
    Subcontractor = 3   // Taşeron/alt yüklenici çalışanları için kullanılır. Örnek: Dış kaynak personel
}
