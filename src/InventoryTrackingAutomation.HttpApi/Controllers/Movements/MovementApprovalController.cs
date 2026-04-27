using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Services.Movements;

using Microsoft.AspNetCore.Authorization;
using InventoryTrackingAutomation.Permissions;
using Microsoft.AspNetCore.Http;

namespace InventoryTrackingAutomation.Controllers.Movements;

/// <summary>
/// Hareket talebi onay işlemleri endpoint'leri.
/// </summary>
[Route("api/movement-requests")]
[ApiExplorerSettings(GroupName = "Movements")]
[Tags("MovementApprovals")]
public class MovementApprovalController : InventoryTrackingAutomationController
{
    private readonly IMovementApprovalAppService _appService;

    public MovementApprovalController(IMovementApprovalAppService appService)
    {
        _appService = appService;
    }

    /// <summary> Hareket talebinin onay geçmişini getirir. </summary>
    [HttpGet("{id}/approvals")]
    [Authorize(InventoryTrackingAutomationPermissions.Workflows.View)]
    public async Task<Result<List<MovementApprovalDto>>> GetApprovals(Guid id)
    {
        var result = await _appService.GetApprovalHistoryAsync(id);
        return result;
    }

    /// <summary> Hareket talebini onaylar veya reddeder. </summary>
    [HttpPost("{id}/process-approval")]
    [Authorize(InventoryTrackingAutomationPermissions.Workflows.Approve)] // Veya Workflows.Reject
    public async Task<Result<MovementApprovalDto>> ProcessApproval(Guid id, [FromBody] ProcessMovementApprovalDto input)
    {
        var result = await _appService.ProcessApprovalAsync(id, input);
        return result;
    }

    /// <summary> Geçerli kullanıcının onaylaması gereken talepleri listeler. </summary>
    [HttpGet("pending-approvals")]
    [Authorize(InventoryTrackingAutomationPermissions.Workflows.View)]
    public async Task<Result<List<PendingApprovalDto>>> GetPendingApprovals()
    {
        var result = await _appService.GetPendingApprovalsAsync();
        return result;
    }
}
