namespace InventoryTrackingAutomation.Dtos.Workflows;

/// <summary>
/// İş akışı adımı tanımı oluşturmak için kullanılan DTO.
/// </summary>
//işlevi: CreateWorkflowStepDefinition verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class CreateWorkflowStepDefinitionDto
{
    public int StepOrder { get; set; }
    public string? RequiredRoleName { get; set; }

    /// <summary>
    /// Onaycı çözümleme anahtarı. Örnekler: "InitiatorManager", "SourceWarehouseManager", "TargetWarehouseManager".
    /// Null/boş ise sadece RequiredRoleName temelli rol bazlı yetki kontrolü yapılır.
    /// </summary>
    public string? ResolverKey { get; set; }
}
