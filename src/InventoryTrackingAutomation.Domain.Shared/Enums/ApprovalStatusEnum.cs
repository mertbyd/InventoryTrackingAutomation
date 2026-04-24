namespace InventoryTrackingAutomation.Enums;

/// <summary>
/// Bir onay adımının karar durumunu tanımlayan enum.
/// </summary>
public enum ApprovalStatusEnum
{
    Pending = 1,   // Onaycı henüz karar vermedi. Örnek: Yeni atanan onay adımı
    Approved = 2,  // Onaycı onayladı. Örnek: Yönetici onay verdi
    Rejected = 3   // Onaycı reddetti. Örnek: Stok yetersizliği nedeniyle reddedildi
}
