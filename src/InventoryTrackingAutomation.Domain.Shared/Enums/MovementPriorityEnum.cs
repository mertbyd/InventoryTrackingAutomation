namespace InventoryTrackingAutomation.Enums;

/// <summary>
/// Malzeme hareket taleplerinin öncelik seviyelerini tanımlayan enum.
/// </summary>
public enum MovementPriorityEnum
{
    Low = 1,     // Düşük öncelikli talep, bekleyebilir. Örnek: Rutin stok yenileme
    Normal = 2,  // Normal öncelikli standart talep. Örnek: Haftalık planlı sevkiyat
    High = 3,    // Yüksek öncelikli, hızlı işlem gerektirir. Örnek: Acil saha ihtiyacı
    Urgent = 4   // Acil talep, anında işlem gerektirir. Örnek: Üretimi durduran kritik eksiklik
}
