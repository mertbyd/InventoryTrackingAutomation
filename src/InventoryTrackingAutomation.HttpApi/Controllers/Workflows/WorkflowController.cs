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
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Controllers.Workflows;

/// <summary>
/// Dinamik İş Akışı (Workflow) operasyonlarının API endpoint'leri.
/// </summary>
[Route("api/workflows")]
[ApiExplorerSettings(GroupName = "Workflows")]
[Tags("Workflows")]
[Authorize]
public class WorkflowController : InventoryTrackingAutomationController, IWorkflowAppService
{
    public WorkflowController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IWorkflowAppService _workflowAppService => LazyGetRequiredService<IWorkflowAppService>();

    /// <summary>
    /// Tanımlı iş akışını ilgili entity için başlatır.
    /// </summary>
    /// <param name="input">İş akışı başlatma bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   EntityId              (Guid) → İş akışına bağlanacak entity Id'si
    ///   WorkflowDefinitionId  (Guid) → Başlatılacak iş akışı tanımı Id'si
    /// }
    /// Response {
    ///   Id                    (Guid)          → İş akışı süreci Id'si
    ///   WorkflowDefinitionId  (Guid)          → Tanım Id'si
    ///   EntityId              (Guid)          → Bağlı entity Id'si
    ///   State                 (WorkflowState) → Süreç durumu
    ///   InitiatorUserId       (Guid)          → Başlatan kullanıcı Id'si
    /// }
    /// </remarks>
    [HttpPost("start")]
    [Authorize(InventoryTrackingAutomationPermissions.Workflows.Approve)]
    public async Task<WorkflowInstanceDto> StartAsync([FromBody] StartWorkflowDto input)
    {
        return await _workflowAppService.StartAsync(input);
    }

    /// <summary>
    /// İş akışı adımı için onay veya red kararını işler.
    /// </summary>
    /// <param name="input">Onay kararı bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   InstanceStepId  (Guid)    → İşlenecek adım Id'si
    ///   IsApproved      (bool)    → Onay kararı (true: onay, false: red)
    ///   Note            (string?) → Karar notu
    /// }
    /// Response {
    ///   Id                        (Guid)               → Adım Id'si
    ///   WorkflowInstanceId        (Guid)               → Bağlı iş akışı süreci Id'si
    ///   WorkflowStepDefinitionId  (Guid)               → Adım tanımı Id'si
    ///   AssignedUserId            (Guid?)              → Atanan kullanıcı Id'si
    ///   ActionTaken               (WorkflowActionType) → Alınan karar
    ///   Note                      (string?)            → Karar notu
    ///   ActionDate                (DateTime?)          → Karar tarihi
    /// }
    /// </remarks>
    [HttpPost("process-approval")]
    [Authorize(InventoryTrackingAutomationPermissions.Workflows.Approve)]
    public async Task<WorkflowInstanceStepDto> ProcessApprovalAsync([FromBody] ProcessApprovalDto input)
    {
        return await _workflowAppService.ProcessApprovalAsync(input);
    }

    /// <summary>
    /// Belirli workflow instance üzerinde onay veya red kararını işler.
    /// </summary>
    /// <param name="instanceId">Workflow instance Id'si.</param>
    /// <param name="input">Onay kararı bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   IsApproved (bool)    → Onay kararı (true: onay, false: red)
    ///   Note       (string?) → Karar notu
    /// }
    /// Response {
    ///   Id                        (Guid)               → Adım Id'si
    ///   WorkflowInstanceId        (Guid)               → Bağlı iş akışı süreci Id'si
    ///   WorkflowStepDefinitionId  (Guid)               → Adım tanımı Id'si
    ///   AssignedUserId            (Guid?)              → Atanan kullanıcı Id'si
    ///   ActionTaken               (WorkflowActionType) → Alınan karar
    ///   Note                      (string?)            → Karar notu
    ///   ActionDate                (DateTime?)          → Karar tarihi
    /// }
    /// </remarks>
    [HttpPost("{instanceId}/process-approval")]
    [Authorize(InventoryTrackingAutomationPermissions.Workflows.Approve)]
    public async Task<WorkflowInstanceStepDto> ProcessInstanceApprovalAsync(
        Guid instanceId,
        [FromBody] ProcessWorkflowInstanceApprovalDto input)
    {
        return await _workflowAppService.ProcessInstanceApprovalAsync(instanceId, input);
    }

    /// <summary>
    /// Oturumdaki kullanıcının bekleyen workflow onay adımlarını getirir.
    /// </summary>
    /// <remarks>
    /// Response {
    ///   WorkflowInstanceStepId  (Guid)     → İş akışı adım Id'si
    ///   WorkflowInstanceId      (Guid)     → Bağlı iş akışı süreci Id'si
    ///   EntityType              (string)   → Bağlı entity türü
    ///   EntityId                (Guid)     → Bağlı entity Id'si
    ///   StepOrder               (int)      → Adım sırası
    ///   StepName                (string)   → Adım adı
    ///   InitiatorUserId         (Guid)     → Süreci başlatan kullanıcı Id'si
    ///   CreatedAt               (DateTime) → Adımın oluşturulma zamanı
    /// }
    /// </remarks>
    [HttpGet("my-pending-approvals")]
    [Authorize(InventoryTrackingAutomationPermissions.Workflows.View)]
    public async Task<List<PendingWorkflowStepDto>> GetMyPendingApprovalsAsync()
    {
        return await _workflowAppService.GetMyPendingApprovalsAsync();
    }

    /// <summary>
    /// Workflow instance geçmişini ve adım kararlarını getirir.
    /// </summary>
    /// <param name="instanceId">Workflow instance Id'si.</param>
    /// <remarks>
    /// Response {
    ///   WorkflowInstanceId    (Guid)     → İş akışı süreci Id'si
    ///   WorkflowDescription   (string?)  → İş akışı açıklaması
    ///   EntityId              (Guid)     → Bağlı entity Id'si
    ///   InitiatorUserId       (Guid)     → Süreci başlatan kullanıcı Id'si
    ///   InitiatorUserName     (string?)  → Başlatan kullanıcı adı
    ///   InitiatorFullName     (string?)  → Başlatan kullanıcı ad-soyad
    ///   CreatedDate           (DateTime) → Sürecin oluşturulma tarihi
    /// }
    /// </remarks>
    [HttpGet("{instanceId}/history")]
    [Authorize(InventoryTrackingAutomationPermissions.Workflows.View)]
    public async Task<WorkflowHistoryDto> GetHistoryAsync(Guid instanceId)
    {
        return await _workflowAppService.GetHistoryAsync(instanceId);
    }
}
