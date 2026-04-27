using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Workflows;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Workflows.Approvers;

// Hareket talebinin hedef (Target) lokasyonunun yöneticisini onaycı olarak çözer.
// Şu an sadece MovementRequest tipinde aktif; başka entity tipleri için ayrı bir strategy eklenmeli.
public class TargetSiteManagerApproverStrategy : IApproverStrategy, ITransientDependency
{
    private readonly IMovementRequestRepository _movementRequestRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IWorkerRepository _workerRepository;

    public TargetSiteManagerApproverStrategy(
        IMovementRequestRepository movementRequestRepository,
        ISiteRepository siteRepository,
        IWorkerRepository workerRepository)
    {
        _movementRequestRepository = movementRequestRepository;
        _siteRepository = siteRepository;
        _workerRepository = workerRepository;
    }

    public string Key => WorkflowResolverKeys.TargetSiteManager;

    public Task<Guid?> ResolveAsync(ApproverContext context)
        => SiteApproverResolver.ResolveAsync(
            context, useSourceSite: false,
            _movementRequestRepository, _siteRepository, _workerRepository);
}
