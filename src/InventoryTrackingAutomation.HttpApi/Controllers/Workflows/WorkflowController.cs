using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Workflows;
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
    public async Task<WorkflowInstanceDto> StartAsync([FromBody] StartWorkflowDto input)
    {
        return await _workflowAppService.StartAsync(input);
    }

    /// <summary>
    /// Belirtilen iş akışı adımında onay veya ret aksiyonunu işler.
    /// </summary>
    [HttpPost("process-approval")]
    public async Task<WorkflowInstanceStepDto> ProcessApprovalAsync([FromBody] ProcessApprovalDto input)
    {
        return await _workflowAppService.ProcessApprovalAsync(input);
    }
}
