using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using System;

namespace InventoryTrackingAutomation.Models.Masters;

/// <summary>
/// Çalışan oluşturma domain modeli — Service'ten Manager'a taşınan veri taşıyıcı.
/// </summary>
public class CreateWorkerModel
{
    public Guid UserId { get; set; }                  // ABP Identity kullanıcı kimliği. Örnek: Guid.NewGuid()
    public string RegistrationNumber { get; set; }    // Sicil numarası. Örnek: "EMP-2024-001"
    public Guid WorkerTypeId { get; set; }    // Çalışan tipi. Örnek: (Lookup)
    public Guid? DepartmentId { get; set; }           // Bağlı departman Id. Örnek: Department Id'si
    public Guid? DefaultWarehouseId { get; set; }          // Varsayılan lokasyon Id. Örnek: Warehouse Id'si
    public Guid? ManagerId { get; set; }              // Yönetici Worker Id. Örnek: Başka bir Worker Id'si
    public bool IsActive { get; set; }                // Aktif mi. Örnek: true
}

