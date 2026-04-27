using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Workflows;

namespace InventoryTrackingAutomation.Managers.Workflows.Approvers;

// Source/Target site manager strategy'leri için ortak çözümleme: MovementRequest → Site → ManagerWorker → User.
internal static class SiteApproverResolver
{
    public static async Task<Guid?> ResolveAsync(
        ApproverContext context,
        bool useSourceSite,
        IMovementRequestRepository movementRequestRepository,
        ISiteRepository siteRepository,
        IWorkerRepository workerRepository)
    {
        if (context.EntityType != WorkflowEntityTypes.MovementRequest)
        {
            return null;
        }

        var request = await movementRequestRepository.FindAsync(context.EntityId);
        if (request == null)
        {
            return null;
        }

        var siteId = useSourceSite ? request.SourceSiteId : request.TargetSiteId;
        if (siteId == Guid.Empty)
        {
            return null;
        }

        var site = await siteRepository.FindAsync(siteId);
        if (site?.ManagerWorkerId == null)
        {
            return null;
        }

        var manager = await workerRepository.FindAsync(site.ManagerWorkerId.Value);
        return manager?.UserId;
    }
}
