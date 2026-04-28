namespace InventoryTrackingAutomation.Enums.Inventory;

/// <summary>
/// Inventory transaction yon ve sebep tipleri (immutable ledger).
/// </summary>
public enum InventoryTransactionTypeEnum
{
    WarehouseToVehicle = 1,    // Depodan araca yukleme (gorev baslangici).
    VehicleToWarehouse = 2,    // Aractan depoya iade (gorev sonu / iptal).
    WarehouseToWarehouse = 3,  // Depo-depo transferi.
    Adjustment = 4             // Manuel envanter sayim duzeltmesi.
}
