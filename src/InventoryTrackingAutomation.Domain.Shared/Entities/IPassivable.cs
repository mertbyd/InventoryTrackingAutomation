namespace InventoryTrackingAutomation.Entities;

/// <summary>
/// Pasife alinabilen master/reference entity'leri icin ortak domain sozlesmesi.
/// </summary>
// islevi: IsActive kolonunu ayni anlamla tasiyan entity'leri isaretler.
// sistemdeki gorevi: Silmeden pasife alma davranisini generic kurallar ve sorgu yardimcilari icin merkezi hale getirir.
public interface IPassivable
{
    bool IsActive { get; set; } // Kaydin operasyonel olarak kullanilabilir olup olmadigini belirler.
}
