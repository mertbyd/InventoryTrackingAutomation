using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Dtos.Tasks;

/// <summary>
/// Envanter gorevi olusturma request DTO'su.
/// </summary>
//işlevi: CreateInventoryTask verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class CreateInventoryTaskDto
{
    public string Code { get; set; }                 // Gorev kodu.
    public string Name { get; set; }                 // Gorev adi.
    public string Region { get; set; }               // Gorev bolgesi.
    public DateTime StartDate { get; set; }          // Baslangic tarihi.
    public DateTime? EndDate { get; set; }           // Bitis tarihi.
    public TaskStatusEnum Status { get; set; } // Gorev durumu.
    public string? Description { get; set; }         // Gorev aciklamasi.
    public Guid? ReturnWarehouseId { get; set; }     // Gorev bitince iadenin donecegi depo.
    public bool IsActive { get; set; }               // Aktiflik bilgisi.
}
