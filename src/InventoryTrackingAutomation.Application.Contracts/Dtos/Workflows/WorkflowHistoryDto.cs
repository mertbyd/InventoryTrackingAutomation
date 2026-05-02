using System;
using System.Collections.Generic;

namespace InventoryTrackingAutomation.Dtos.Workflows;

//işlevi: WorkflowHistory verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class WorkflowHistoryDto
{
    /// <summary>
    /// WorkflowInstanceId alanı.
    /// </summary>
    public Guid WorkflowInstanceId { get; set; }

    public string WorkflowDefinitionName { get; set; } = string.Empty;
    /// <summary>
    /// WorkflowDescription alanı.
    /// </summary>
    public string? WorkflowDescription { get; set; }

    public string EntityType { get; set; } = string.Empty;
    /// <summary>
    /// EntityId alanı.
    /// </summary>
    public Guid EntityId { get; set; }

    public string State { get; set; } = string.Empty;
    /// <summary>
    /// InitiatorUserId alanı.
    /// </summary>
    public Guid InitiatorUserId { get; set; }
    /// <summary>
    /// InitiatorUserName alanı.
    /// </summary>
    public string? InitiatorUserName { get; set; }
    /// <summary>
    /// InitiatorFullName alanı.
    /// </summary>
    public string? InitiatorFullName { get; set; }
    /// <summary>
    /// CreatedDate alanı.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    public List<WorkflowHistoryStepDto> Steps { get; set; } = new();
}
