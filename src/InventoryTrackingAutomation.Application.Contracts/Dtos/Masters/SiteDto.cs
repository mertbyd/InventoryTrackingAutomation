using System;
using Volo.Abp.Application.Dtos;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Masters;

/// <summary>
/// Lokasyon response DTO'su — GetAll ve GetById operasyonlarında döner.
/// </summary>
public class SiteDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; }            // Lokasyon kodu. Örnek: "SITE-WH01"
    public string Name { get; set; }            // Lokasyon adı. Örnek: "İstanbul Merkez Depo"
    public SiteTypeEnum SiteType { get; set; }  // Lokasyon tipi. Örnek: SiteTypeEnum.Warehouse
    public string? Address { get; set; }        // Fiziksel adres. Örnek: "Atatürk Cad. No:5"
    public Guid? ManagerWorkerId { get; set; }  // Saha/Depo yöneticisi Worker Id'si.
    public bool IsActive { get; set; }          // Aktif mi. Örnek: true
}
