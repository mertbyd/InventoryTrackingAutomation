using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Managers.Masters;
using InventoryTrackingAutomation.Workflows;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Workflows.Approvers;

// İş akışını başlatan kullanıcının doğrudan yöneticisini onaycı olarak çözer.
// Worker zinciri çözümlemesini WorkerManager'a delege eder (DRY — diğer yerlerle tek doğruluk kaynağı).
public class InitiatorManagerApproverStrategy : IApproverStrategy, ITransientDependency
{
    private readonly WorkerManager _workerManager;

    public InitiatorManagerApproverStrategy(WorkerManager workerManager)
    {
        _workerManager = workerManager;
    }

    public string Key => WorkflowResolverKeys.InitiatorManager;

    public Task<Guid?> ResolveAsync(ApproverContext context)
        => _workerManager.GetManagerUserIdAsync(context.InitiatorUserId);
}
