namespace InventoryTrackingAutomation.Enums;

/// <summary>
/// Malzeme hareket taleplerinin durum akışını tanımlayan enum.
/// </summary>
public enum MovementStatusEnum
{
    Pending = 1,    // Talep oluşturulmuş, ilk onaycıya atanmayı bekliyor. Örnek: Yeni oluşturulan talep
    InReview = 2,   // Talep onay sürecinde, bir veya daha fazla onaycı kararını bekleniyor. Örnek: 1. onaycı onayladı, 2. onaycı bekliyor
    Approved = 3,   // Tüm onaycılar onayladı, sevkiyat hazırlanıyor. Örnek: Tüm onaylar tamamlandı
    Shipped = 4,    // Malzeme yola çıkmış, teslim bekliyor. Örnek: Araçta yüklü
    Completed = 5,  // Teslim tamamlanmış, kapatılmış. Örnek: Alıcı teslim aldı
    Cancelled = 6,  // Talep iptal edilmiş. Örnek: İhtiyaç ortadan kalktı
    Rejected = 7    // Talep reddedilmiş. Örnek: Stok yetersizliği nedeniyle reddedildi
}
