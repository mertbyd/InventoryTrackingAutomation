using System;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Masters;

/// <summary>
/// Depo response DTO'su.
/// </summary>
//işlevi: Warehouse verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class WarehouseDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; } = default!;      // Deponun benzersiz is kodu.
    public string Name { get; set; } = default!;      // Depo adi.
    public string? Address { get; set; }              // Depoya ulasim icin adres bilgisi.
    public Guid? ManagerWorkerId { get; set; }        // Depodan sorumlu calisan.
    public bool IsActive { get; set; }                // Operasyonlarda kullanilabilirlik durumu.
}
