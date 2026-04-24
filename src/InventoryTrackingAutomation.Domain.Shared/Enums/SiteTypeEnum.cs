namespace InventoryTrackingAutomation.Enums;

/// <summary>
/// Sistemdeki lokasyon/saha tiplerini tanımlayan enum.
/// </summary>
public enum SiteTypeEnum
{
    Warehouse = 1,   // Depo tipi lokasyonlar için kullanılır. Örnek: Merkez Depo
    Field = 2,       // Açık alan/şantiye tipi lokasyonlar için kullanılır. Örnek: İnşaat Sahası A
    Vehicle = 3,     // Araç bazlı hareketli lokasyonlar için kullanılır. Örnek: 34 ABC 123 plakalı araç
    Personnel = 4    // Personel bazlı lokasyonlar için kullanılır. Örnek: Teknisyen Ali Yılmaz
}
