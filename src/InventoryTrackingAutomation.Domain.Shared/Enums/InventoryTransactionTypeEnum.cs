namespace InventoryTrackingAutomation.Enums;

/// <summary>
/// Envanter stok hareketinin islem tipleri.
/// </summary>
public enum InventoryTransactionTypeEnum
{
    WarehouseToVehicle = 1, // Depodan araca yukleme.
    VehicleToWarehouse = 2, // Aractan depoya iade.
    WarehouseToWarehouse = 3, // Depodan depoya transfer.
    VehicleToVehicle = 4 // Aractan araca transfer.
}
