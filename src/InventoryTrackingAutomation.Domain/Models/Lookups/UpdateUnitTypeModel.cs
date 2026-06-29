namespace InventoryTrackingAutomation.Models.Lookups;

// islevi: UnitType guncelleme verisini AppService'ten domain manager'a tasir.
// sistemdeki gorevi: Lookup guncelleme is kurallarinin DTO'dan bagimsiz calismasini saglar.
public class UpdateUnitTypeModel
{
    /// <summary>
    /// Kodu.
    /// </summary>
    public string Code { get; set; } = default!;
    /// <summary>
    /// Adý.
    /// </summary>
    public string Name { get; set; } = default!;
    /// <summary>
    /// Açýklamasý.
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// Aktif mi.
    /// </summary>
    public bool IsActive { get; set; }
}

