namespace InventoryTrackingAutomation.Notifications;

/// <summary>
/// SSE bildirim akisinin typed ayarlari; appsettings "Sse" bolumunden okunur.
/// </summary>
// islevi: Heartbeat araligi ve baglanti kanali tamponunu tek noktada tanimlar.
// sistemdeki gorevi: SSE endpoint'i ve baglanti yoneticisi davranisini koddan degil config'den alir; ElasticsearchOptions kalibinin SSE karsiligidir.
public class SseOptions
{
    public const string SectionName = "Sse";

    /// <summary>
    /// Sessiz baglantida ara sunucular (proxy/load balancer) timeout uygulamasin diye atilan heartbeat araligi (saniye).
    /// </summary>
    public int HeartbeatSeconds { get; set; } = 15;

    /// <summary>
    /// Baglanti basina bildirim kanalinin tamponu; dolarsa en eski bildirim dusurulur, yavas client sunucuyu sisirmez.
    /// </summary>
    public int ChannelCapacity { get; set; } = 64;
}
