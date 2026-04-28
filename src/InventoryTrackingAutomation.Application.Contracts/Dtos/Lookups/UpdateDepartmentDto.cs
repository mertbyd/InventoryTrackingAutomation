namespace InventoryTrackingAutomation.Dtos.Lookups;

/// <summary>
/// Departman güncelleme request DTO'su.
/// </summary>
//işlevi: UpdateDepartment verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class UpdateDepartmentDto
{
    public string Code { get; set; }       // Departman kodu. Örnek: "DEP-IT"
    public string Name { get; set; }       // Departman adı. Örnek: "Bilgi Teknolojileri"
}
