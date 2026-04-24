namespace InventoryTrackingAutomation.Enums;

/// <summary>
/// Stok hareket tiplerini ve yönlerini tanımlayan enum.
/// </summary>
public enum StockMovementTypeEnum
{
    In = 1,               // Depoya giriş hareketi. Örnek: Tedarikçiden mal alımı
    Out = 2,              // Depodan çıkış hareketi. Örnek: Sahaya malzeme gönderimi
    Reserve = 3,          // Stok rezervasyon hareketi. Örnek: Onaylanan talep için stok ayırma
    Unreserve = 4,        // Rezervasyon iptali hareketi. Örnek: İptal edilen talep rezervasyonu serbest bırakma
    TransferOut = 5,      // Lokasyonlar arası transfer çıkış hareketi. Örnek: Depo A'dan çıkış
    TransferIn = 6,       // Lokasyonlar arası transfer giriş hareketi. Örnek: Depo B'ye giriş
    Adjustment = 7,       // Manuel stok düzeltme hareketi. Örnek: Sayım farkı düzeltmesi
    AllocationOut = 8,    // Personel/araç tahsisi çıkış hareketi. Örnek: Teknisyene malzeme tahsisi
    AllocationReturn = 9  // Personel/araç tahsisinden iade giriş hareketi. Örnek: Kullanılmayan malzeme iadesi
}
