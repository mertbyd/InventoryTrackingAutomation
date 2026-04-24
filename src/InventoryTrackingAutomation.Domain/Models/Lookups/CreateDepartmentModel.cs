namespace InventoryTrackingAutomation.Models.Lookups;

/// <summary>
/// Departman oluşturma domain modeli — Service'ten Manager'a taşınan veri taşıyıcı.
/// </summary>
public class CreateDepartmentModel
{
    public string Code { get; set; }        // Departman kodu. Örnek: "DEP-IT"
    public string Name { get; set; }        // Departman adı. Örnek: "Bilgi Teknolojileri"
}
