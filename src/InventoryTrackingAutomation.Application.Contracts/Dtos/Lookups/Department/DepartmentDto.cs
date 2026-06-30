using System;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Lookups;

//işlevi: Department verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class DepartmentDto : EntityDto<Guid>
{
    /// <summary>
    /// Departman kodu. Örnek: &quot;DEP-IT&quot;
    /// </summary>
    public string Code { get; set; }       // Departman kodu. Örnek: "DEP-IT"
    /// <summary>
    /// Departman adı. Örnek: &quot;Bilgi Teknolojileri&quot;
    /// </summary>
    public string Name { get; set; }       // Departman adı. Örnek: "Bilgi Teknolojileri"
}

