using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Workflows;
using InventoryTrackingAutomation.Interface.Workflows;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Interface.Masters;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Workflows;

/// <summary>
/// Workflow motorunun onaycı çözümleyici kancasının (hook) varsayılan implementasyonu.
/// </summary>
public class DefaultWorkflowApproverResolver : IWorkflowApproverResolver, ITransientDependency
{
    private readonly IMovementRequestRepository _movementRequestRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IWorkerRepository _workerRepository;

    public DefaultWorkflowApproverResolver(
        IMovementRequestRepository movementRequestRepository,
        ISiteRepository siteRepository,
        IWorkerRepository workerRepository)
    {
        _movementRequestRepository = movementRequestRepository;
        _siteRepository = siteRepository;
        _workerRepository = workerRepository;
    }

    public async Task<Guid?> ResolveApproverAsync(string entityType, Guid entityId, WorkflowStepDefinition stepDefinition)
    {
        if (string.IsNullOrEmpty(stepDefinition.ResolverKey))
        {
            return null; // Çözümleyici yoksa null dön.
        }

        if (stepDefinition.ResolverKey == "SourceSiteManager" && entityType == "MovementRequest")
        {
            var request = await _movementRequestRepository.FindAsync(entityId);
            if (request != null && request.SourceSiteId != Guid.Empty)
            {
                var site = await _siteRepository.FindAsync(request.SourceSiteId);
                if (site != null && site.ManagerWorkerId.HasValue)
                {
                    // Site.ManagerWorkerId bir Worker Id'sidir. Bize UserId lazım olabilir.
                    var worker = await _workerRepository.FindAsync(site.ManagerWorkerId.Value);
                    return worker?.UserId;
                }
            }
        }

        return null;
    }
}
