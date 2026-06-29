using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using System;

namespace InventoryTrackingAutomation.Models.Tasks;

/// <summary>
/// Envanter gorevi olusturma domain modeli.
/// </summary>
public class CreateInventoryTaskModel
{
    public InventoryTaskTypeEnum Type { get; set; }       // Gorev surec tipi.
    public string Code { get; set; }                 // Gorev kodu. Ornek: "TASK-IZMIR-001"
    public string Name { get; set; }                 // Gorev adi.
    public string? Region { get; set; }               // Gorev bolgesi.
    public DateTime StartDate { get; set; }          // Baslangic tarihi.
    public DateTime? EndDate { get; set; }           // Bitis tarihi.
    public TaskStatusEnum Status { get; set; } // Gorev durumu.
    public string? Description { get; set; }         // Gorev aciklamasi.
    public Guid SourceWarehouseId { get; set; }      // Operasyon malzemesinin cikacagi depo.
    public Guid? TargetWarehouseId { get; set; }     // Depo transferlerinde hedef depo.
    public Guid? ReturnWarehouseId { get; set; }     // Gorev bitince iadenin donecegi depo.
}
