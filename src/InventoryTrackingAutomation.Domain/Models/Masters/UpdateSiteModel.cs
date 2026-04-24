using System;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Models.Masters;

/// <summary>
/// Lokasyon güncelleme domain modeli — Service'ten Manager'a taşınan veri taşıyıcı.
/// </summary>
public class UpdateSiteModel
{
    public string Code { get; set; }                  // Lokasyon kodu. Örnek: "SITE-WH01"
    public string Name { get; set; }                  // Lokasyon adı. Örnek: "İstanbul Merkez Depo"
    public SiteTypeEnum SiteType { get; set; }        // Lokasyon tipi. Örnek: SiteTypeEnum.Warehouse
    public string? Address { get; set; }              // Fiziksel adres. Örnek: "Atatürk Cad. No:5"
    public Guid? LinkedVehicleId { get; set; }        // Lokasyona tahsis edilmiş araç Id. Örnek: Vehicle Id'si
    public Guid? LinkedWorkerId { get; set; }         // Lokasyona tahsis edilmiş çalışan Id. Örnek: Worker Id'si
    public bool IsActive { get; set; }                // Aktif mi. Örnek: true
}
