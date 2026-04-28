using System;

namespace InventoryTrackingAutomation.Dtos.Workflows;

/// <summary>
/// İş akışını başlatmak için kullanılan DTO.
/// </summary>
//işlevi: StartWorkflow verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class StartWorkflowDto
{
    /// <summary>
    /// Hangi entity türü için iş akışı başlatılacak (Örn: "MovementRequest").
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// İş akışına tabi olan entity'nin Id'si.
    /// </summary>
    public Guid EntityId { get; set; }

    /// <summary>
    /// Başlatılacak iş akışı şablonunun Id'si.
    /// </summary>
    public Guid WorkflowDefinitionId { get; set; }
}
