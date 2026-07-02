using System;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Masters;

//işlevi: Vehicle verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class VehicleDto : EntityDto<Guid>
{
    /// <summary>
    /// Plaka numarası. Örnek: &quot;34 ABC 123&quot;
    /// </summary>
    public string PlateNumber { get; set; }

    /// <summary>
    /// Araç tipi Id (Lookup FK).
    /// </summary>
    public Guid VehicleTypeId { get; set; }

    /// <summary>
    /// Araç tipi adı (VehicleType navigation'ından doldurulur).
    /// </summary>
    public string VehicleTypeName { get; set; }
}
