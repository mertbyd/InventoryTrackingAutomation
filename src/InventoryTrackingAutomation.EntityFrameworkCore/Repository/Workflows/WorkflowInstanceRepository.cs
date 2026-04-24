using InventoryTrackingAutomation.Entities.Workflows;
using InventoryTrackingAutomation.Interface.Workflows;
using Volo.Abp.EntityFrameworkCore;

namespace InventoryTrackingAutomation.Repository.Workflows;

/// <summary>
/// WorkflowInstance entity'si için EF Core repository implementasyonu.
/// </summary>
public class WorkflowInstanceRepository : BaseRepository<WorkflowInstance>, IWorkflowInstanceRepository
{
    public WorkflowInstanceRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
