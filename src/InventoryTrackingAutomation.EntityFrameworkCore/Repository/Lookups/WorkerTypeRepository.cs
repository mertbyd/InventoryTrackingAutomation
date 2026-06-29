using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;

namespace InventoryTrackingAutomation.Repository.Lookups;

/// <summary>
/// WorkerType entity'si için EF Core repository implementasyonu.
/// </summary>
public class WorkerTypeRepository : BaseRepository<WorkerType>, IWorkerTypeRepository
{
    public WorkerTypeRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
