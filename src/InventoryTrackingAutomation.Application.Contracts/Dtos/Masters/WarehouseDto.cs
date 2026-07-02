using System;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Masters;

//işlevi: Warehouse verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class WarehouseDto : EntityDto<Guid>
{
    public string Code { get; set; } = default!;      // Deponun benzersiz is kodu.
    public string Name { get; set; } = default!;      // Depo adi.

    /// <summary>
    /// Depoya ulasim icin adres bilgisi.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Depodan sorumlu calisan Id.
    /// </summary>
    public Guid? ManagerWorkerId { get; set; }

    /// <summary>
    /// Depodan sorumlu calisan adi (ManagerWorker navigation'ından doldurulur).
    /// </summary>
    public string ManagerWorkerName { get; set; }
}
