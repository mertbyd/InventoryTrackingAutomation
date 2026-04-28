using System;

namespace InventoryTrackingAutomation.Dtos.Masters;

/// <summary>
/// Depo olusturma request DTO'su.
/// </summary>
public class CreateWarehouseDto
{
    public string Code { get; set; } = default!;      // Deponun benzersiz is kodu.
    public string Name { get; set; } = default!;      // Depo adi.
    public string? Address { get; set; }              // Depoya ulasim icin adres bilgisi.
    public Guid? ManagerWorkerId { get; set; }        // Depodan sorumlu calisan.
    public bool IsActive { get; set; }                // Operasyonlarda kullanilabilirlik durumu.
}
