using System;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Models.Masters;

/// <summary>
/// Çalışan güncelleme domain modeli — Service'ten Manager'a taşınan veri taşıyıcı.
/// </summary>
public class UpdateWorkerModel
{
    public Guid UserId { get; set; }                  // ABP Identity kullanıcı kimliği. Örnek: Guid.NewGuid()
    public string RegistrationNumber { get; set; }    // Sicil numarası. Örnek: "EMP-2024-001"
    public WorkerTypeEnum WorkerType { get; set; }    // Çalışan tipi. Örnek: WorkerTypeEnum.BlueCollar
    public Guid? DepartmentId { get; set; }           // Bağlı departman Id. Örnek: Department Id'si
    public Guid? DefaultWarehouseId { get; set; }          // Varsayılan lokasyon Id. Örnek: Warehouse Id'si
    public Guid? ManagerId { get; set; }              // Yönetici Worker Id. Örnek: Başka bir Worker Id'si
    public bool IsActive { get; set; }                // Aktif mi. Örnek: true
}
