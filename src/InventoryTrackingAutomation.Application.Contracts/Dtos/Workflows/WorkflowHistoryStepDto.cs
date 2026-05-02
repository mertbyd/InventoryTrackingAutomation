using System;
using System.Collections.Generic;

namespace InventoryTrackingAutomation.Dtos.Workflows;

//işlevi: WorkflowHistoryStep verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class WorkflowHistoryStepDto
{
    /// <summary>
    /// StepOrder alanı.
    /// </summary>
    public int StepOrder { get; set; }

    public string StepName { get; set; } = string.Empty;
    /// <summary>
    /// RequiredRoleName alanı.
    /// </summary>
    public string? RequiredRoleName { get; set; }
    /// <summary>
    /// ResolverKey alanı.
    /// </summary>
    public string? ResolverKey { get; set; }

    public string StepStatus { get; set; } = string.Empty;
    /// <summary>
    /// CreatedDate alanı.
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    public List<WorkflowHistoryApproverDto> Approvers { get; set; } = new();
}
