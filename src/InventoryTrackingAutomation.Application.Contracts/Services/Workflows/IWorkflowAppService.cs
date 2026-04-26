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
    /// Mevcut kullanıcının onaylaması bekleyen tüm iş akışı adımlarını döner.
    /// Entity türünden bağımsız çalışır — herhangi bir workflow tüketicisi için kullanılabilir.
    /// </summary>
    Task<List<PendingWorkflowStepDto>> GetMyPendingApprovalsAsync();
}
