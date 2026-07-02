namespace InventoryTrackingAutomation.Search;

/// <summary>
/// Elasticsearch index adlarini merkezi olarak tutar.
/// </summary>
// islevi: Arama index'lerinin adlarini tek noktadan tanimlar.
// sistemdeki gorevi: Index adi hicbir serviste string literal olarak yazilmaz; her kullanim buradan okur.
public static class SearchIndexNames
{
    // Tum inventory index'lerinin ortak on eki; Kibana/ES tarafinda gruplamayi kolaylastirir.
    private const string Prefix = "inventory";

    // Urun metin aramasi icin kullanilan index.
    public const string Products = $"{Prefix}-products";
}
