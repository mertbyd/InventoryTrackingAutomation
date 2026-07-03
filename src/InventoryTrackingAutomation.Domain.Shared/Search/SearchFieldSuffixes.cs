namespace InventoryTrackingAutomation.Search;

/// <summary>
/// Elasticsearch multi-field mapping'lerindeki alt alan adi sabitleri.
/// </summary>
// islevi: Text alanlarin alt alan adlarini tek noktada tanimlar; string literal dagitilmaz.
// sistemdeki gorevi: Birebir eslesme sorgulari analiz edilmemis ".keyword" alt alanini bu sabit uzerinden hedefler.
public static class SearchFieldSuffixes
{
    /// <summary>
    /// Text alanlarin analiz edilmemis keyword alt alani; birebir eslesme sorgularinda kullanilir.
    /// </summary>
    public const string Keyword = "keyword";
}
