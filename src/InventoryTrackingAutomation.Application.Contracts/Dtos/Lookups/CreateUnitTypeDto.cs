namespace InventoryTrackingAutomation.Dtos.Lookups;

// islevi: Yeni UnitType lookup kaydi olusturmak icin gerekli veriyi tasir.
// sistemdeki gorevi: Enum yerine DB'den yonetilen olcu birimi referanslarini API uzerinden ekletir.
public class CreateUnitTypeDto
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
    /// <summary>
    /// Aktif mi.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

