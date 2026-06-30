using System;
using InventoryTrackingAutomation.Managers;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Managers.Masters;
using InventoryTrackingAutomation.Workflows;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Workflows.Approvers;

// İş akışını başlatan kullanıcının doğrudan yöneticisini onaycı olarak çözer.
// Worker zinciri çözümlemesini WorkerManager'a delege eder (DRY — diğer yerlerle tek doğruluk kaynağı).
//işlevi: InitiatorManagerApproverStrategy.cs etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class InitiatorManagerApproverStrategy : InventoryTrackingAutomationLazyService, IApproverStrategy, ITransientDependency
{
    public InitiatorManagerApproverStrategy(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private WorkerManager _workerManager => LazyGetRequiredService<WorkerManager>();



    public string Key => WorkflowResolverKeys.InitiatorManager;

    //işlevi: Etki alanı kuralını veya validasyonunu işletir.
    //sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public Task<Guid?> ResolveAsync(ApproverContext context)
        => _workerManager.GetManagerUserIdAsync(context.InitiatorUserId);
}
