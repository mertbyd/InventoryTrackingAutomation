namespace InventoryTrackingAutomation.Enums;

/// <summary>
/// Sevkiyat sürecinin durum akışını tanımlayan enum.
/// </summary>
public enum ShipmentStatusEnum
{
    Preparing = 1,   // Sevkiyat hazırlanıyor, yükleme devam ediyor. Örnek: Araç yükleniyor
    InTransit = 2,   // Sevkiyat yolda, teslimat bekleniyor. Örnek: Araç hedefe gidiyor
    Delivered = 3,   // Sevkiyat teslim edildi, tamamlandı. Örnek: Alıcı malı teslim aldı
    Cancelled = 4    // Sevkiyat iptal edildi. Örnek: Araç arızası nedeniyle iptal
}
