namespace InventoryTrackingAutomation.Dtos.Lookups;

// islevi: Var olan WorkerType lookup kaydinin guncellenecek alanlarini tasir.
// sistemdeki gorevi: Calisan tipi referans verisinin kod, ad, aciklama ve aktiflik bilgisini gunceller.
public class UpdateWorkerTypeDto
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

