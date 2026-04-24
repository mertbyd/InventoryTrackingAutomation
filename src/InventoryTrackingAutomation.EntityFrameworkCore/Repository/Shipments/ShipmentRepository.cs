using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Shipments;
using InventoryTrackingAutomation.Interface.Shipments;

namespace InventoryTrackingAutomation.Repository.Shipments;

/// <summary>
/// Shipment entity'si için EF Core repository implementasyonu.
/// </summary>
public class ShipmentRepository : BaseRepository<Shipment>, IShipmentRepository
{
    public ShipmentRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
