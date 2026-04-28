using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Tasks;

/// <summary>
/// Envanter gorevi response DTO'su.
/// </summary>
//işlevi: InventoryTask verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class InventoryTaskDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; }                 // Gorev kodu.
    public string Name { get; set; }                 // Gorev adi.
    public string Region { get; set; }               // Gorev bolgesi.
    public DateTime StartDate { get; set; }          // Baslangic tarihi.
    public DateTime? EndDate { get; set; }           // Bitis tarihi.
    public TaskStatusEnum Status { get; set; } // Gorev durumu.
    public string? Description { get; set; }         // Gorev aciklamasi.
    public bool IsActive { get; set; }               // Aktiflik bilgisi.
}
