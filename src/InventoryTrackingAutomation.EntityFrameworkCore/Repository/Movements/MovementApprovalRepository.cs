using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

    public override async Task<IQueryable<MovementApproval>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).Include(x => x.MovementRequest).Include(x => x.ApproverWorker);
    }
}


