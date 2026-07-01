using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Common;

/// <summary>
/// References sözlüğünü tek noktadan sağlayan ortak response DTO base'i.
/// </summary>
// işlevi: EntityDto<TKey> üzerine FK display referanslarını taşıyan References sözlüğünü ekler.
// sistemdeki görevi: DTO'ların FK başına ayrı Name/Code alanı tanımlamadan display taşımasını sağlar.
public abstract class EnrichedEntityDto<TKey> : EntityDto<TKey>, IHasDisplayReferences
{
    /// <summary>
    /// FK property adı -> ekran karşılığı eşlemesi. Enrichment mekanizması doldurur.
    /// </summary>
    public IDictionary<string, DisplayLabelDto> References { get; set; } = new Dictionary<string, DisplayLabelDto>();
}
