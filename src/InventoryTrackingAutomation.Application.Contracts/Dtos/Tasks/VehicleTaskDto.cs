using System;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Tasks;

/// <summary>
/// Arac-gorev atamasi response DTO'su.
/// </summary>
//işlevi: VehicleTask verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class VehicleTaskDto : FullAuditedEntityDto<Guid>
{
    public Guid VehicleId { get; set; }       // Arac Id'si.
    public Guid InventoryTaskId { get; set; } // Gorev Id'si.
    public DateTime AssignedAt { get; set; }  // Atama zamani.
    public DateTime? ReleasedAt { get; set; } // Birakma zamani.
    public bool IsActive { get; set; }        // Aktiflik bilgisi.
}
