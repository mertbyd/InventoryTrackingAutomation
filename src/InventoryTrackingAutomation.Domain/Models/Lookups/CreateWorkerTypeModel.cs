namespace InventoryTrackingAutomation.Models.Lookups;

// islevi: WorkerType olusturma verisini AppService'ten domain manager'a tasir.
// sistemdeki gorevi: API DTO'sunu domain kural katmanindan ayirir.
public class CreateWorkerTypeModel
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

