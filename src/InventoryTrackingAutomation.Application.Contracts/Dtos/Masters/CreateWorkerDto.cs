using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Masters;

/// <summary>
/// Çalışan oluşturma request DTO'su.
/// </summary>
//işlevi: CreateWorker verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class CreateWorkerDto
{
    public Guid UserId { get; set; }                  // ABP Identity kullanıcı kimliği.
    public string RegistrationNumber { get; set; }    // Sicil numarası. Örnek: "EMP-2024-001"
    public WorkerTypeEnum WorkerType { get; set; }    // Çalışan tipi. Örnek: WorkerTypeEnum.BlueCollar
    public Guid? DepartmentId { get; set; }           // Bağlı departman Id.
    public Guid? DefaultWarehouseId { get; set; }          // Varsayılan lokasyon Id.
    public Guid? ManagerId { get; set; }              // Yönetici Worker Id.
    public bool IsActive { get; set; }                // Aktif mi. Örnek: true
}
