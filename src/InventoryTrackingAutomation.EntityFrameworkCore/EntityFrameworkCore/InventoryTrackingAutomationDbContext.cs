using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Entities.Workflows;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace InventoryTrackingAutomation.EntityFrameworkCore;

[ConnectionStringName(InventoryTrackingAutomationDbProperties.ConnectionStringName)]
public class InventoryTrackingAutomationDbContext : AbpDbContext<InventoryTrackingAutomationDbContext>, IInventoryTrackingAutomationDbContext
{
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Department> Departments { get; set; }

    public DbSet<Product> Products { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
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

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
