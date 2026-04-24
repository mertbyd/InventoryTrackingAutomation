using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Interface.Movements;

namespace InventoryTrackingAutomation.Repository.Movements;

/// <summary>
/// MovementRequest entity'si için EF Core repository implementasyonu.
/// </summary>
public class MovementRequestRepository : BaseRepository<MovementRequest>, IMovementRequestRepository
{
    public MovementRequestRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
