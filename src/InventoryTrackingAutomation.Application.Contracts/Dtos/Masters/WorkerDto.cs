using System;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Masters;

//işlevi: Worker verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class WorkerDto : EntityDto<Guid>
{
    /// <summary>
    /// ABP Identity kullanıcı kimliği.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Sicil numarası. Örnek: &quot;EMP-2024-001&quot;
    /// </summary>
    public string RegistrationNumber { get; set; }

    /// <summary>
    /// Çalışan tipi Id (Lookup FK).
    /// </summary>
    public Guid WorkerTypeId { get; set; }

    /// <summary>
    /// Çalışan tipi adı (WorkerType navigation'ından doldurulur).
    /// </summary>
    public string WorkerTypeName { get; set; }

    /// <summary>
    /// Bağlı departman Id.
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// Bağlı departman adı (Department navigation'ından doldurulur).
    /// </summary>
    public string DepartmentName { get; set; }

    /// <summary>
    /// Varsayılan depo Id.
    /// </summary>
    public Guid? DefaultWarehouseId { get; set; }

    /// <summary>
    /// Varsayılan depo adı (DefaultWarehouse navigation'ından doldurulur).
    /// </summary>
    public string DefaultWarehouseName { get; set; }

    /// <summary>
    /// Yönetici Worker Id.
    /// </summary>
    public Guid? ManagerId { get; set; }

    /// <summary>
    /// Yönetici sicil numarası (Manager navigation'ından doldurulur; Worker'da isim alanı yoktur).
    /// </summary>
    public string ManagerName { get; set; }
}
