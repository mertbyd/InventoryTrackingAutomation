using System;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Lookups;

/// <summary>
/// Departman response DTO'su — GetAll ve GetById operasyonlarında döner.
/// </summary>
public class DepartmentDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; }       // Departman kodu. Örnek: "DEP-IT"
    public string Name { get; set; }       // Departman adı. Örnek: "Bilgi Teknolojileri"
}
