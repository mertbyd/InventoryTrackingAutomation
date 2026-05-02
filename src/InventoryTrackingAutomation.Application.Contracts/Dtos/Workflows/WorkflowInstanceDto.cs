using System;
using InventoryTrackingAutomation.Enums.Workflows;

namespace InventoryTrackingAutomation.Dtos.Workflows;

//işlevi: WorkflowInstance verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class WorkflowInstanceDto
{
    /// <summary>
    /// Id alanı.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// WorkflowDefinitionId alanı.
    /// </summary>
    public Guid WorkflowDefinitionId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    /// <summary>
    /// EntityId alanı.
    /// </summary>
    public Guid EntityId { get; set; }
    /// <summary>
    /// State alanı.
    /// </summary>
    public WorkflowState State { get; set; }
    /// <summary>
    /// InitiatorUserId alanı.
    /// </summary>
    public Guid InitiatorUserId { get; set; }
}
