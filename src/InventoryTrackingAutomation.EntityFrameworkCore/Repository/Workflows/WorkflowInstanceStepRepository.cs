using InventoryTrackingAutomation.Entities.Workflows;
using InventoryTrackingAutomation.Interface.Workflows;
using Volo.Abp.EntityFrameworkCore;

namespace InventoryTrackingAutomation.Repository.Workflows;

/// <summary>
/// WorkflowInstanceStep entity'si için EF Core repository implementasyonu.
/// </summary>
public class WorkflowInstanceStepRepository : BaseRepository<WorkflowInstanceStep>, IWorkflowInstanceStepRepository
{
    public WorkflowInstanceStepRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
