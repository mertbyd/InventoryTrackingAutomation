using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using System;

namespace InventoryTrackingAutomation.Dtos.Masters;

//işlevi: CreateWorker verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class CreateWorkerDto
{
    /// <summary>
    /// ABP Identity kullanıcı kimliği.
    /// </summary>
    public Guid UserId { get; set; }                  // ABP Identity kullanıcı kimliği.
    /// <summary>
    /// Sicil numarası. Örnek: &quot;EMP-2024-001&quot;
    /// </summary>
    public string RegistrationNumber { get; set; }    // Sicil numarası. Örnek: "EMP-2024-001"
    /// <summary>
    /// Çalışan tipi. Örnek: (Lookup)
    /// </summary>
    public Guid WorkerTypeId { get; set; }    // Çalışan tipi. Örnek: (Lookup)
    /// <summary>
    /// Bağlı departman Id.
    /// </summary>
    public Guid? DepartmentId { get; set; }           // Bağlı departman Id.
    /// <summary>
    /// Varsayılan lokasyon Id.
    /// </summary>
    public Guid? DefaultWarehouseId { get; set; }          // Varsayılan lokasyon Id.
    /// <summary>
    /// Yönetici Worker Id.
    /// </summary>
    public Guid? ManagerId { get; set; }              // Yönetici Worker Id.
    /// <summary>
    /// Aktif mi. Örnek: true
    /// </summary>
    public bool IsActive { get; set; }                // Aktif mi. Örnek: true
}

