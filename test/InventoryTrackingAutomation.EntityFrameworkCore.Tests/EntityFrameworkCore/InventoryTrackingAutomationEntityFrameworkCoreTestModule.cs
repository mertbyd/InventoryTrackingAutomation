using System;
using InventoryTrackingAutomation.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.EntityFrameworkCore;

[DependsOn(
    typeof(InventoryTrackingAutomationTestBaseModule),
    typeof(InventoryTrackingAutomationApplicationModule),
    typeof(InventoryTrackingAutomationEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqliteModule)
    )]
public class InventoryTrackingAutomationEntityFrameworkCoreTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAlwaysDisableUnitOfWorkTransaction();

        var sqliteConnection = CreateDatabaseAndGetConnection();
        context.Services.AddSingleton(sqliteConnection);

        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(abpDbContextConfigurationContext =>
            {
                abpDbContextConfigurationContext.DbContextOptions.UseSqlite(sqliteConnection.ConnectionString);
            });
        });
    }

    private static SqliteConnection CreateDatabaseAndGetConnection()
    {
        var connection = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        connection.Open();

        using (var dbContext = new InventoryTrackingAutomationDbContext(
                   new DbContextOptionsBuilder<InventoryTrackingAutomationDbContext>().UseSqlite(connection).Options))
        {
            dbContext.GetService<IRelationalDatabaseCreator>().CreateTables();
        }

        return connection;
    }
}
