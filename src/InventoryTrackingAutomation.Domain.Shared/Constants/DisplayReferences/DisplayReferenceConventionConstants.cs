namespace InventoryTrackingAutomation.Constants.DisplayReferences;

/// <summary>
/// Display reference convention motorunun kullandigi property isim sabitlerini merkezi olarak tutar.
/// </summary>
public static class DisplayReferenceConventionConstants
{
    /// <summary>
    /// DTO property adinin FK olarak degerlendirilmesi icin bitmesi gereken ektir. Ornek: "VehicleTaskId".
    /// </summary>
    public const string IdSuffix = "Id";

    /// <summary>
    /// Entity'nin kurumsal kod display alan adidir.
    /// </summary>
    public const string CodeField = "Code";

    /// <summary>
    /// Entity'nin kullaniciya gorunen ana ad display alan adidir.
    /// </summary>
    public const string NameField = "Name";

    /// <summary>
    /// Name yoksa alternatif gorunen baslik display alan adidir.
    /// </summary>
    public const string TitleField = "Title";

    /// <summary>
    /// Code/Name/Title yoksa takip numarasi gibi alanlari yakalamak icin kullanilan son ektir. Ornek: "PlateNumber".
    /// </summary>
    public const string NumberSuffix = "Number";

    /// <summary>
    /// ListResultDto/PagedResultDto gibi sonuc sarmalayicilarindaki koleksiyon property adidir.
    /// </summary>
    public const string ItemsProperty = "Items";
}
