namespace InventoryTrackingAutomation.Enums;

/// <summary>
/// Hareket talebinin operasyonel surec tipini tanimlar.
/// </summary>
public enum MovementRequestTypeEnum
{
    WarehouseToWarehouse = 1,   // Depodan depoya arac uzerinden transfer.
    WarehouseToTask = 2,        // Depodan saha gorevine/araca malzeme cikisi.
    TaskReturnToWarehouse = 3   // Gorev bitisi sonrasi aractan depoya kontrollu iade.
}
