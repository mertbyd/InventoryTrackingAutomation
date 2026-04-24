using System;
using Volo.Abp.Application.Dtos;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Masters;

/// <summary>
/// Çalışan response DTO'su — GetAll ve GetById operasyonlarında döner.
/// </summary>
public class WorkerDto : FullAuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }                  // ABP Identity kullanıcı kimliği.
    public string RegistrationNumber { get; set; }    // Sicil numarası. Örnek: "EMP-2024-001"
    public WorkerTypeEnum WorkerType { get; set; }    // Çalışan tipi. Örnek: WorkerTypeEnum.BlueCollar
    public Guid? DepartmentId { get; set; }           // Bağlı departman Id.
    public Guid? DefaultSiteId { get; set; }          // Varsayılan lokasyon Id.
    public Guid? ManagerId { get; set; }              // Yönetici Worker Id.
    public bool IsActive { get; set; }                // Aktif mi. Örnek: true
}
