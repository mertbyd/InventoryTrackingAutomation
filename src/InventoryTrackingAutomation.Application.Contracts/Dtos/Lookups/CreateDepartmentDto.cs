namespace InventoryTrackingAutomation.Dtos.Lookups;

/// <summary>
/// Departman oluşturma request DTO'su.
/// </summary>
//işlevi: CreateDepartment verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class CreateDepartmentDto
{
    public string Code { get; set; }       // Departman kodu. Örnek: "DEP-IT"
    public string Name { get; set; }       // Departman adı. Örnek: "Bilgi Teknolojileri"
}
