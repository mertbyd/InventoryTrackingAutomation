using System;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Lookups;

// islevi: WorkerType lookup verisini API katmanina tasir.
// sistemdeki gorevi: Calisan tipi seceneklerinin id, kod, ad ve aktiflik bilgisini standart DTO ile sunar.
public class WorkerTypeDto : EntityDto<Guid>
{
    /// <summary>
    /// Kodu.
    /// </summary>
    public string Code { get; set; } = default!;
    /// <summary>
    /// Adý.
    /// </summary>
    public string Name { get; set; } = default!;
    /// <summary>
    /// Açýklamasý.
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// Aktif mi.
    /// </summary>
    public bool IsActive { get; set; }
}

