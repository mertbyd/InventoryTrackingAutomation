namespace InventoryTrackingAutomation.Enums.Tasks;

/// <summary>
/// Operasyon isinin hangi surec ailesine ait oldugunu tanimlar.
/// </summary>
public enum InventoryTaskTypeEnum
{
    WarehouseTransfer = 1, // Depodan depoya sevkiyat isi.
    FieldOperation = 2 // Saha/gorev operasyon isi.
}
