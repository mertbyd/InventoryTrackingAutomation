using System;

namespace InventoryTrackingAutomation.Application.Caching;

/// <summary>
/// Okuma metodunun cache key sablonunu ve yasam suresini tasir.
/// </summary>
// islevi: Cache'lenecek servis metodunu attribute ile isaretler.
// sistemdeki gorevi: AppService icindeki manuel Redis okuma/yazma tekrarini interceptor katmanina tasimak icin metadata saglar.
[AttributeUsage(AttributeTargets.Method)]
public sealed class InventoryCacheAttribute : Attribute
{
    /// <summary>
    /// Cache sablonu ve dakika bazli absolute expiration bilgisini alir.
    /// </summary>
    public InventoryCacheAttribute(string keyTemplate, int absoluteExpirationMinutes = 10)
    {
        KeyTemplate = keyTemplate;
        AbsoluteExpirationMinutes = absoluteExpirationMinutes;
    }

    /// <summary>
    /// Ornek: stock-summary:{id}. {id} metot parametresi ile degistirilir.
    /// </summary>
    public string KeyTemplate { get; }

    /// <summary>
    /// Cache kaydinin Redis uzerinde kac dakika tutulacagini belirler.
    /// </summary>
    public int AbsoluteExpirationMinutes { get; }
}
