using System;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Masters;

/// <summary>
/// Depo, saha veya personel gibi stok lokasyonlarını temsil eden master data entity'si.
/// </summary>
public class Site : FullAuditedEntity<Guid>
{
    public string Code { get; set; }           // Lokasyonun benzersiz kodu. Örnek: "SITE-WH01"
    public string Name { get; set; }           // Lokasyonun adı. Örnek: "İstanbul Merkez Depo"
    public SiteTypeEnum SiteType { get; set; } // Lokasyon tipi. Örnek: SiteTypeEnum.Warehouse
    public string? Address { get; set; }       // Fiziksel adresi. Örnek: "Atatürk Cad. No:5 İstanbul"
    public Guid? ManagerWorkerId { get; set; } // Bu deponun/şantiyenin yöneticisinin Worker Id'si.
    public bool IsActive { get; set; }         // Lokasyon aktif mi. Örnek: true

    protected Site() { }
    public Site(Guid id) : base(id) { }
}
