namespace InventoryTrackingAutomation.Dtos.Lookups;

// işlevi: Tüm Lookup CRUD DTO'ları için ortak alan tanımını tek yerde toplar.
// sistemdeki görevi: UnitType, VehicleType, WorkerType gibi lookup DTO'larındaki
//                    kod tekrarını ortadan kaldırır; yeni bir lookup eklendiğinde
//                    sadece bu base'den kalıtmak yeterlidir.
public abstract class LookupCommonDto
{
    /// <summary>
    /// Benzersiz sistem kodu. Örnek: "VAN", "PIECE", "WHITE_COLLAR"
    /// </summary>
    public string Code { get; set; } = default!;

    /// <summary>
    /// Kullanıcıya gösterilen ad. Örnek: "Panelvan", "Adet", "Beyaz Yaka"
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// İsteğe bağlı açıklama metni.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Kaydın aktif olup olmadığını gösterir.
    /// </summary>
    public bool IsActive { get; set; }
}
