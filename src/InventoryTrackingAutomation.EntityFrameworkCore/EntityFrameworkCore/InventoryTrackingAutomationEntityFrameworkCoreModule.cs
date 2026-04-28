using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Entities.Stock;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Entities.Workflows;
using InventoryTrackingAutomation.Repository.Lookups;
using InventoryTrackingAutomation.Repository.Masters;
using InventoryTrackingAutomation.Repository.Movements;
using InventoryTrackingAutomation.Repository.Stock;
using InventoryTrackingAutomation.Repository.Tasks;
using InventoryTrackingAutomation.Repository.Workflows;

namespace InventoryTrackingAutomation.EntityFrameworkCore;

[DependsOn(
    typeof(InventoryTrackingAutomationDomainModule),
    typeof(AbpEntityFrameworkCoreModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(AbpOpenIddictEntityFrameworkCoreModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule)
)]
public class InventoryTrackingAutomationEntityFrameworkCoreModule : AbpModule
{
    static InventoryTrackingAutomationEntityFrameworkCoreModule()
    {
        ConfigureAbpSchemasStatic();
    }

    private static void ConfigureAbpSchemasStatic()
    {
        // ABP modülleri için default schema ayarları
        Volo.Abp.Identity.AbpIdentityDbProperties.DbSchema = "abp";
        Volo.Abp.PermissionManagement.AbpPermissionManagementDbProperties.DbSchema = "abp";
        Volo.Abp.SettingManagement.AbpSettingManagementDbProperties.DbSchema = "abp";
        Volo.Abp.AuditLogging.AbpAuditLoggingDbProperties.DbSchema = "abp";
        Volo.Abp.FeatureManagement.AbpFeatureManagementDbProperties.DbSchema = "abp";
        Volo.Abp.TenantManagement.AbpTenantManagementDbProperties.DbSchema = "abp";
        Volo.Abp.OpenIddict.AbpOpenIddictDbProperties.DbSchema = "openiddict";
    }

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        ConfigureAllSchemas(configuration);
    }

    public static void ConfigureAllSchemas(IConfiguration configuration)
    {
        ConfigureAbpSchemasStatic();

        var schemas = configuration.GetSection("EntityFrameworkCore:Schemas");
        if (!schemas.Exists()) return;

        // ABP modül schema override'ları
        var identitySchema = schemas["Volo.Abp.Identity"];
        if (!string.IsNullOrEmpty(identitySchema))
            Volo.Abp.Identity.AbpIdentityDbProperties.DbSchema = identitySchema;

        var permissionSchema = schemas["Volo.Abp.PermissionManagement"];
        if (!string.IsNullOrEmpty(permissionSchema))
            Volo.Abp.PermissionManagement.AbpPermissionManagementDbProperties.DbSchema = permissionSchema;

        var settingSchema = schemas["Volo.Abp.SettingManagement"];
        if (!string.IsNullOrEmpty(settingSchema))
            Volo.Abp.SettingManagement.AbpSettingManagementDbProperties.DbSchema = settingSchema;

        var auditSchema = schemas["Volo.Abp.AuditLogging"];
        if (!string.IsNullOrEmpty(auditSchema))
            Volo.Abp.AuditLogging.AbpAuditLoggingDbProperties.DbSchema = auditSchema;

        var openIddictSchema = schemas["Volo.Abp.OpenIddict"];
        if (!string.IsNullOrEmpty(openIddictSchema))
            Volo.Abp.OpenIddict.AbpOpenIddictDbProperties.DbSchema = openIddictSchema;

        var tenantSchema = schemas["Volo.Abp.TenantManagement"];
        if (!string.IsNullOrEmpty(tenantSchema))
            Volo.Abp.TenantManagement.AbpTenantManagementDbProperties.DbSchema = tenantSchema;

        var featureSchema = schemas["Volo.Abp.FeatureManagement"];
        if (!string.IsNullOrEmpty(featureSchema))
            Volo.Abp.FeatureManagement.AbpFeatureManagementDbProperties.DbSchema = featureSchema;

        // Inventory modül schema override'ları
        var lookupSchema = schemas["Inventory.Lookup"];
        if (!string.IsNullOrEmpty(lookupSchema))
            InventoryTrackingAutomationDbProperties.LookupSchema = lookupSchema;

        var masterSchema = schemas["Inventory.Master"];
        if (!string.IsNullOrEmpty(masterSchema))
            InventoryTrackingAutomationDbProperties.MasterSchema = masterSchema;

        var inventorySchema = schemas["Inventory.Inventory"];
        if (!string.IsNullOrEmpty(inventorySchema))
            InventoryTrackingAutomationDbProperties.InventorySchema = inventorySchema;

        var operationSchema = schemas["Inventory.Operation"];
        if (!string.IsNullOrEmpty(operationSchema))
            InventoryTrackingAutomationDbProperties.OperationSchema = operationSchema;

        var movementSchema = schemas["Inventory.Movement"];
        if (!string.IsNullOrEmpty(movementSchema))
            InventoryTrackingAutomationDbProperties.MovementSchema = movementSchema;

        var workflowSchema = schemas["Inventory.Workflow"];
        if (!string.IsNullOrEmpty(workflowSchema))
            InventoryTrackingAutomationDbProperties.WorkflowSchema = workflowSchema;
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbContextOptions>(options =>
        {
            options.UseNpgsql();
            options.Configure(ctx =>
            {
                ctx.DbContextOptions.UseSnakeCaseNamingConvention();
            });
        });

        context.Services.AddAbpDbContext<InventoryTrackingAutomationDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);

            options.AddRepository<ProductCategory, ProductCategoryRepository>();
            options.AddRepository<Department, DepartmentRepository>();

            options.AddRepository<Product, ProductRepository>();
            options.AddRepository<Warehouse, WarehouseRepository>();
            options.AddRepository<Vehicle, VehicleRepository>();
            options.AddRepository<Worker, WorkerRepository>();

            options.AddRepository<StockLocation, StockLocationRepository>();
            options.AddRepository<InventoryTransaction, InventoryTransactionRepository>();

            options.AddRepository<MovementRequest, MovementRequestRepository>();
            options.AddRepository<MovementRequestLine, MovementRequestLineRepository>();
            options.AddRepository<MovementApproval, MovementApprovalRepository>();

            options.AddRepository<InventoryTask, InventoryTaskRepository>();
            options.AddRepository<VehicleTask, VehicleTaskRepository>();

            options.AddRepository<WorkflowDefinition, WorkflowDefinitionRepository>();
            options.AddRepository<WorkflowInstance, WorkflowInstanceRepository>();
            options.AddRepository<WorkflowInstanceStep, WorkflowInstanceStepRepository>();
        });
    }
}
