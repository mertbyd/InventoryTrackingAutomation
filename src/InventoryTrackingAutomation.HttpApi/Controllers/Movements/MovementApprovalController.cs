using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Services.Movements;
using Volo.Abp.DependencyInjection;

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
    public MovementApprovalController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IMovementApprovalAppService _appService => LazyGetRequiredService<IMovementApprovalAppService>();

    /// <summary>
    /// Hareket talebinin onay geçmişini getirir.
    /// </summary>
    /// <param name="id">Hareket talebi Id'si.</param>
    /// <remarks>
    /// Response {
    ///   Id                (Guid)      → Onay kaydı Id'si
    ///   MovementRequestId (Guid)      → Bağlı hareket talebi Id'si
    ///   ApproverWorkerId  (Guid)      → Onaylayan çalışan Id'si
    ///   StepOrder         (int)       → Onay adım sırası
    ///   Status            (string)    → Onay durumu
    ///   DecidedAt         (DateTime?) → Karar tarihi
    ///   Note              (string)    → Onay notu
    /// }
    /// </remarks>
    [HttpGet("{id}/approvals")]
    [Authorize(InventoryTrackingAutomationPermissions.Workflows.View)]
    public async Task<Result<List<MovementApprovalDto>>> GetApprovals(Guid id)
    {
        var result = await _appService.GetApprovalHistoryAsync(id);
        return result;
    }

    /// <summary>
    /// Hareket talebi için onay veya red kararını işler.
    /// </summary>
    /// <param name="id">Hareket talebi Id'si.</param>
    /// <param name="input">Onay kararı bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   IsApproved (bool)   → Onay kararı (true: onay, false: red)
    ///   Note       (string) → Karar notu
    /// }
    /// Response {
    ///   Id                (Guid)      → Onay kaydı Id'si
    ///   MovementRequestId (Guid)      → Bağlı hareket talebi Id'si
    ///   ApproverWorkerId  (Guid)      → Onaylayan çalışan Id'si
    ///   StepOrder         (int)       → Onay adım sırası
    ///   Status            (string)    → Onay durumu
    ///   DecidedAt         (DateTime?) → Karar tarihi
    ///   Note              (string)    → Onay notu
    /// }
    /// </remarks>
    [HttpPost("{id}/process-approval")]
    [Authorize(InventoryTrackingAutomationPermissions.Workflows.Approve)]
    public async Task<Result<MovementApprovalDto>> ProcessApproval(Guid id, [FromBody] ProcessMovementApprovalDto input)
    {
        var result = await _appService.ProcessApprovalAsync(id, input);
        return result;
    }

    /// <summary>
    /// Oturumdaki kullanıcının bekleyen hareket talebi onaylarını getirir.
    /// </summary>
    /// <remarks>
    /// Response {
    ///   MovementRequestId       (Guid)                  → Hareket talebi Id'si
    ///   WorkflowInstanceStepId  (Guid)                  → İş akışı adım Id'si
    ///   RequestNumber           (string)                → Talep numarası
    ///   SourceWarehouseName     (string)                → Kaynak depo adı
    ///   TargetWarehouseName     (string)                → Hedef depo adı
    ///   CurrentStepOrder        (int)                   → Mevcut onay adımı sırası
    ///   CurrentStepName         (string)                → Mevcut adım adı
    ///   CreatedAt               (DateTime)              → Talep oluşturma tarihi
    ///   PlannedDate             (DateTime)              → Planlanan teslim tarihi
    ///   RequestNote             (string)                → Talep gerekçesi
    ///   Priority                (MovementPriorityEnum)  → Öncelik
    /// }
    /// </remarks>
    [HttpGet("pending-approvals")]
    [Authorize(InventoryTrackingAutomationPermissions.Workflows.View)]
    public async Task<Result<List<PendingApprovalDto>>> GetPendingApprovals()
    {
        var result = await _appService.GetPendingApprovalsAsync();
        return result;
    }
}
