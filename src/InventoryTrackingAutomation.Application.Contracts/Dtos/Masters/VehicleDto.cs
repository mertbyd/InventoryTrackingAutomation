using System;
using Volo.Abp.Application.Dtos;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Masters;

/// <summary>
/// Araç response DTO'su — GetAll ve GetById operasyonlarında döner.
/// </summary>
//işlevi: Vehicle verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class VehicleDto : FullAuditedEntityDto<Guid>
{
    public string PlateNumber { get; set; }           // Plaka numarası. Örnek: "34 ABC 123"
    public VehicleTypeEnum VehicleType { get; set; } // Araç tipi. Örnek: VehicleTypeEnum.Van
    public bool IsActive { get; set; }               // Aktif mi. Örnek: true
}
