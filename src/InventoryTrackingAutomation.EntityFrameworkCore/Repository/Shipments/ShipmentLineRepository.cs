using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Shipments;
using InventoryTrackingAutomation.Interface.Shipments;

namespace InventoryTrackingAutomation.Repository.Shipments;

/// <summary>
/// ShipmentLine entity'si için EF Core repository implementasyonu.
/// </summary>
public class ShipmentLineRepository : BaseRepository<ShipmentLine>, IShipmentLineRepository
{
    public ShipmentLineRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
