using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Entities.Stock;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Entities.Workflows;

namespace InventoryTrackingAutomation.EntityFrameworkCore;

[ConnectionStringName(InventoryTrackingAutomationDbProperties.ConnectionStringName)]
public class InventoryTrackingAutomationDbContext : AbpDbContext<InventoryTrackingAutomationDbContext>, IInventoryTrackingAutomationDbContext
{
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Department> Departments { get; set; }

    public DbSet<Product> Products { get; set; }
    public DbSet<Site> Sites { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Worker> Workers { get; set; }

    public DbSet<StockLocation> StockLocations { get; set; }
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

    public DbSet<MovementRequest> MovementRequests { get; set; }
    public DbSet<MovementRequestLine> MovementRequestLines { get; set; }
    public DbSet<MovementApproval> MovementApprovals { get; set; }

    public DbSet<InventoryTask> InventoryTasks { get; set; }
    public DbSet<VehicleTask> VehicleTasks { get; set; }

    public DbSet<WorkflowDefinition> WorkflowDefinitions { get; set; }
    public DbSet<WorkflowStepDefinition> WorkflowStepDefinitions { get; set; }
    public DbSet<WorkflowInstance> WorkflowInstances { get; set; }
    public DbSet<WorkflowInstanceStep> WorkflowInstanceSteps { get; set; }

    public InventoryTrackingAutomationDbContext(DbContextOptions<InventoryTrackingAutomationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
