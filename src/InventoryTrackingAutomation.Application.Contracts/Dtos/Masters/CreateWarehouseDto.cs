using System;

namespace InventoryTrackingAutomation.Dtos.Masters;

//işlevi: CreateWarehouse verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class CreateWarehouseDto
{
    public string Code { get; set; } = default!;      // Deponun benzersiz is kodu.
    public string Name { get; set; } = default!;      // Depo adi.
    /// <summary>
    /// Depoya ulasim icin adres bilgisi.
    /// </summary>
    public string? Address { get; set; }              // Depoya ulasim icin adres bilgisi.
    /// <summary>
    /// Depodan sorumlu calisan.
    /// </summary>
    public Guid? ManagerWorkerId { get; set; }        // Depodan sorumlu calisan.
    /// <summary>
    /// Operasyonlarda kullanilabilirlik durumu.
    /// </summary>
    public bool IsActive { get; set; }                // Operasyonlarda kullanilabilirlik durumu.
}
