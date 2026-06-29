namespace InventoryTrackingAutomation.Dtos.Lookups;

// islevi: Yeni WorkerType lookup kaydi olusturmak icin gerekli veriyi tasir.
// sistemdeki gorevi: Enum yerine DB'den yonetilen calisan tipi referanslarini API uzerinden ekletir.
public class CreateWorkerTypeDto
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

