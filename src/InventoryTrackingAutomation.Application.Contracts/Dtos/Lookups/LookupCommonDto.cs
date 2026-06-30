using System;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Lookups;

public abstract class LookupCommonDto : EntityDto<Guid>
{
    /// <summary>
    /// Kodu.
    /// </summary>
    public string Code { get; set; } = default!;
    
    /// <summary>
    /// Adı.
    /// </summary>
    public string Name { get; set; } = default!;
    
    /// <summary>
    /// Açıklaması.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Aktif mi.
    /// </summary>
    public bool IsActive { get; set; }
}
