using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Workflows;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Workflows;

/// <summary>
/// Dinamik iş akışlarını dış dünyaya açan uygulama servisi.
/// </summary>
public interface IWorkflowAppService : IApplicationService
{
    /// <summary>
    /// Yeni bir iş akışı süreci başlatır.
    /// </summary>
    Task<WorkflowInstanceDto> StartAsync(StartWorkflowDto input);

    /// <summary>
    /// Belirtilen iş akışı adımında onay veya ret aksiyonunu işler.
    /// </summary>
    Task<WorkflowInstanceStepDto> ProcessApprovalAsync(ProcessApprovalDto input);

    /// <summary>
    /// Belirtilen iş akışı sürecindeki mevcut bekleyen adımı onaylar veya reddeder.
    /// </summary>
    /// <param name="instanceId">WorkflowInstance.Id</param>
    /// <param name="input">Onay/red bilgisi.</param>
    Task<WorkflowInstanceStepDto> ProcessInstanceApprovalAsync(Guid instanceId, ProcessWorkflowInstanceApprovalDto input);

    /// <summary>
    /// Mevcut kullanıcının onaylaması bekleyen tüm iş akışı adımlarını döner.
    /// Entity türünden bağımsız çalışır — herhangi bir workflow tüketicisi için kullanılabilir.
    /// </summary>
    Task<List<PendingWorkflowStepDto>> GetMyPendingApprovalsAsync();

    /// <summary>
    /// Belirtilen iş akışı süreci için tam tarihçeyi döner.
    /// Geçmiş + mevcut + gelecek adımları içerir; UI'da "talep detay > onay zinciri" görünümü için.
    /// </summary>
    /// <param name="instanceId">WorkflowInstance.Id</param>
    Task<WorkflowHistoryDto> GetHistoryAsync(Guid instanceId);
}
