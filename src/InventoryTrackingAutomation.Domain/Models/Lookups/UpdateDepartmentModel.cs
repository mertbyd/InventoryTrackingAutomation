namespace InventoryTrackingAutomation.Models.Lookups;

/// <summary>
/// Departman güncelleme domain modeli — Service'ten Manager'a taşınan veri taşıyıcı.
/// </summary>
public class UpdateDepartmentModel
{
    public string Code { get; set; }        // Departman kodu. Örnek: "DEP-IT"
    public string Name { get; set; }        // Departman adı. Örnek: "Bilgi Teknolojileri"
}
