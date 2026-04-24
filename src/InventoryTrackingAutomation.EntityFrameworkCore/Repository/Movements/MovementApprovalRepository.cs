using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Interface.Movements;

namespace InventoryTrackingAutomation.Repository.Movements;

/// <summary>
/// MovementApproval entity'si için EF Core repository implementasyonu.
/// </summary>
public class MovementApprovalRepository : BaseRepository<MovementApproval>, IMovementApprovalRepository
{
    public MovementApprovalRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
