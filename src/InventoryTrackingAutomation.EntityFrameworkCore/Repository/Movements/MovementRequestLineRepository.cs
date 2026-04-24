using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Interface.Movements;

namespace InventoryTrackingAutomation.Repository.Movements;

/// <summary>
/// MovementRequestLine entity'si için EF Core repository implementasyonu.
/// </summary>
public class MovementRequestLineRepository : BaseRepository<MovementRequestLine>, IMovementRequestLineRepository
{
    public MovementRequestLineRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
