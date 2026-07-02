namespace InventoryTrackingAutomation.Search;

/// <summary>
/// Elasticsearch baglanti ve arama ayarlarinin typed options sinifi.
/// </summary>
// islevi: appsettings "Elasticsearch" bolumunu compile-time guvenli tasir.
// sistemdeki gorevi: Url, fuzziness ve batch degerleri kodda literal yazilmaz; tek kaynak konfigurasyondur.
public class ElasticsearchOptions
{
    public const string SectionName = "Elasticsearch"; // appsettings bolum adi; binding tek noktadan yapilir.

    public string? Url { get; set; } // Elasticsearch endpoint'i.
    public string Fuzziness { get; set; } = "AUTO"; // Metin aramasinda kullanilacak fuzziness degeri.
    public int ReindexBatchSize { get; set; } = 500; // Reindex sirasinda tek bulk istegine yazilacak dokuman sayisi.
}
