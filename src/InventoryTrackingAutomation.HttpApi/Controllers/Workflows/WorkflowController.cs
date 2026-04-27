using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Workflows;
using InventoryTrackingAutomation.Permissions;
using InventoryTrackingAutomation.Services.Workflows;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Volo.Abp;

namespace InventoryTrackingAutomation.Controllers.Workflows;

/// <summary>
/// Dinamik İş Akışı (Workflow) operasyonlarının API endpoint'leri.
/// </summary>
[Route("api/workflows")]
[ApiExplorerSettings(GroupName = "Workflows")]
[Tags("Workflows")]
[Authorize] // İşlemlerin tümünde giriş yapmış kullanıcı kimliği (CurrentUser.Id) gerekir.
public class WorkflowController : InventoryTrackingAutomationController, IWorkflowAppService
{
    private readonly IWorkflowAppService _workflowAppService;

    public WorkflowController(IWorkflowAppService workflowAppService)
    {
        _workflowAppService = workflowAppService;
    }

    /// <summary>
    /// Yeni bir iş akışı süreci başlatır.
    /// </summary>
    [HttpPost("start")]
    [Authorize]
    public async Task<WorkflowInstanceDto> StartAsync([FromBody] StartWorkflowDto input)
    {
        return await _workflowAppService.StartAsync(input);
    }

    /// <summary>
    /// Belirtilen iş akışı adımında onay veya ret aksiyonunu işler.
    /// </summary>
    [HttpPost("process-approval")]
    [Authorize(InventoryTrackingAutomationPermissions.Workflows.Approve)]
    public async Task<WorkflowInstanceStepDto> ProcessApprovalAsync([FromBody] ProcessApprovalDto input)
    {
        return await _workflowAppService.ProcessApprovalAsync(input);
    }

    /// <summary>
    /// Belirtilen iş akışı sürecindeki mevcut bekleyen adımı onaylar veya reddeder.
    /// </summary>
    [HttpPost("{instanceId}/process-approval")]
    [Authorize(InventoryTrackingAutomationPermissions.Workflows.Approve)]
    public async Task<WorkflowInstanceStepDto> ProcessInstanceApprovalAsync(
        Guid instanceId,
        [FromBody] ProcessWorkflowInstanceApprovalDto input)
    {
        return await _workflowAppService.ProcessInstanceApprovalAsync(instanceId, input);
    }

    /// <summary>
    /// Mevcut kullanıcının onaylaması bekleyen tüm iş akışı adımlarını döner.
    /// </summary>
    [HttpGet("my-pending-approvals")]
    [Authorize]
    public async Task<List<PendingWorkflowStepDto>> GetMyPendingApprovalsAsync()
    {
        return await _workflowAppService.GetMyPendingApprovalsAsync();
    }

    /// <summary>
    /// Belirtilen iş akışı süreci için tam tarihçeyi döner (kim başlattı, hangi adım onaylandı/bekliyor/kaldı).
    /// </summary>
    [HttpGet("{instanceId}/history")]
    [Authorize]
    public async Task<WorkflowHistoryDto> GetHistoryAsync(Guid instanceId)
    {
        return await _workflowAppService.GetHistoryAsync(instanceId);
    }
}
