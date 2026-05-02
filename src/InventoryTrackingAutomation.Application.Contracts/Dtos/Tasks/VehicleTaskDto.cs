using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Tasks;

//işlevi: VehicleTask verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class VehicleTaskDto : EntityDto<Guid>
{
    /// <summary>
    /// Arac Id&apos;si.
    /// </summary>
    public Guid VehicleId { get; set; }       // Arac Id'si.
    /// <summary>
    /// Operasyon isi Id&apos;si.
    /// </summary>
    public Guid TaskId { get; set; } // Operasyon isi Id'si.
    /// <summary>
    /// Atamadan sorumlu calisan Id&apos;si.
    /// </summary>
    public Guid ResponsibleWorkerId { get; set; } // Atamadan sorumlu calisan Id'si.
    /// <summary>
    /// Atama zamani.
    /// </summary>
    public DateTime AssignedAt { get; set; }  // Atama zamani.
    /// <summary>
    /// Birakma zamani.
    /// </summary>
    public DateTime? ReleasedAt { get; set; } // Birakma zamani.
    /// <summary>
    /// Arac-gorev kalemleri. Detayli sorgularda dolu gelir.
    /// </summary>
    public List<VehicleTaskLineDto>? Lines { get; set; } // Arac-gorev kalemleri.
}
