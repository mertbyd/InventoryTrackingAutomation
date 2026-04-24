using System;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Masters;

/// <summary>
/// Sistemdeki çalışanları ve organizasyon hiyerarşisini temsil eden master data entity'si.
/// </summary>
public class Worker : FullAuditedEntity<Guid>
{
    public Guid UserId { get; set; }                 // ABP Identity kullanıcı kimliği. Örnek: Guid.NewGuid()
    public string RegistrationNumber { get; set; }   // Sicil/personel numarası. Örnek: "EMP-2024-001"
    public WorkerTypeEnum WorkerType { get; set; }   // Çalışan tipi. Örnek: WorkerTypeEnum.BlueCollar
    public Guid? DepartmentId { get; set; }          // Bağlı olduğu departman. Örnek: Department Id'si
    public Guid? DefaultSiteId { get; set; }         // Varsayılan lokasyonu. Örnek: Site Id'si
    public Guid? ManagerId { get; set; }             // Yöneticisinin kimliği (self-ref hiyerarşi). Örnek: Başka bir Worker Id'si
    public bool IsActive { get; set; }               // Çalışan aktif mi. Örnek: true

    protected Worker() { }
    public Worker(Guid id) : base(id) { }
}
