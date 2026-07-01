using System.Collections.Generic;

namespace InventoryTrackingAutomation.Dtos.Common;

/// <summary>
/// FK alanlarının ekran karşılıklarını (display label) taşıyabilen DTO sözleşmesi.
/// </summary>
// işlevi: DTO'nun References sözlüğü üzerinden FK -> ekran metni eşlemesini taşıdığını bildirir.
// sistemdeki görevi: Enrichment mekanizmasının hangi DTO'ları zenginleştireceğini işaretler (marker + taşıyıcı).
public interface IHasDisplayReferences
{
    /// <summary>
    /// FK property adı -> ekran karşılığı. Örnek: References["VehicleTaskId"] = { Id, Code, Name }.
    /// </summary>
    IDictionary<string, DisplayLabelDto> References { get; }
}
