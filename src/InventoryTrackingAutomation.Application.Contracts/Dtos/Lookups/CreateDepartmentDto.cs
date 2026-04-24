namespace InventoryTrackingAutomation.Dtos.Lookups;

/// <summary>
/// Departman oluşturma request DTO'su.
/// </summary>
public class CreateDepartmentDto
{
    public string Code { get; set; }       // Departman kodu. Örnek: "DEP-IT"
    public string Name { get; set; }       // Departman adı. Örnek: "Bilgi Teknolojileri"
}
