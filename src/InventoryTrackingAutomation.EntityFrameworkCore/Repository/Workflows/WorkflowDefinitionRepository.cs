using InventoryTrackingAutomation.Entities.Workflows;
using InventoryTrackingAutomation.Interface.Workflows;
using Volo.Abp.EntityFrameworkCore;

namespace InventoryTrackingAutomation.Repository.Workflows;

/// <summary>
/// WorkflowDefinition entity'si için EF Core repository implementasyonu.
/// </summary>
public class WorkflowDefinitionRepository : BaseRepository<WorkflowDefinition>, IWorkflowDefinitionRepository
{
    public WorkflowDefinitionRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
