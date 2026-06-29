using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities;
using InventoryTrackingAutomation.Entities.Lookups;

namespace InventoryTrackingAutomation.EntityFrameworkCore;

[ConnectionStringName(InventoryTrackingAutomationDbProperties.ConnectionStringName)]
public interface IInventoryTrackingAutomationDbContext : IEfCoreDbContext
{
    DbSet<ProductCategory> ProductCategories { get; }
    DbSet<Department> Departments { get; }
    DbSet<VehicleType> VehicleTypes { get; }
    DbSet<WorkerType> WorkerTypes { get; }
    DbSet<UnitType> UnitTypes { get; }
}
