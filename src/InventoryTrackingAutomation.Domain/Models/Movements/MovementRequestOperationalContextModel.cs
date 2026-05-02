using System;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Tasks;

namespace InventoryTrackingAutomation.Models.Movements;

/// <summary>
/// MovementRequest icin join ile cozulmus operasyon baglamini tasir.
/// </summary>
public class MovementRequestOperationalContextModel
{
    public Guid MovementRequestId { get; set; } // Hareket talebi Id'si.
    public Guid RequestedByWorkerId { get; set; } // Talebi baslatan calisan Id'si.
    public Guid SourceWarehouseId { get; set; } // Kaynak depo Id'si.
    public Guid? TargetWarehouseId { get; set; } // Hedef depo Id'si.
    public Guid TaskId { get; set; } // Bagli operasyon isi Id'si.
    public InventoryTaskTypeEnum TaskType { get; set; } // Operasyon isinin surec tipi.
    public TaskStatusEnum TaskStatus { get; set; } // Operasyon isinin durumu.
    public Guid? ReturnWarehouseId { get; set; } // Saha operasyonu iade deposu Id'si.
    public Guid VehicleTaskId { get; set; } // Arac-operasyon atamasi Id'si.
    public Guid VehicleId { get; set; } // Operasyona atanmis arac Id'si.
    public Guid ResponsibleWorkerId { get; set; } // Arac atamasindan sorumlu calisan Id'si.
    public Guid? ParentMovementRequestId { get; set; } // Iade hareketinin ana talep Id'si.
    public MovementStatusEnum Status { get; set; } // Hareket talebinin operasyonel durumu.

    public bool IsReturnFlow => ParentMovementRequestId.HasValue; // Iade hareketi olup olmadigini turetir.
    public bool IsWarehouseTransfer => TaskType == InventoryTaskTypeEnum.WarehouseTransfer; // Depo transferi olup olmadigini turetir.
    public bool IsFieldOperation => TaskType == InventoryTaskTypeEnum.FieldOperation; // Saha operasyonu olup olmadigini turetir.
}
