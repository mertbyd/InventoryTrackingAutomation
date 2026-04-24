using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace InventoryTrackingAutomation.EntityFrameworkCore;

public class InventoryTrackingAutomationHttpApiHostMigrationsDbContextFactory : IDesignTimeDbContextFactory<InventoryTrackingAutomationHttpApiHostMigrationsDbContext>
{
    public InventoryTrackingAutomationHttpApiHostMigrationsDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<InventoryTrackingAutomationHttpApiHostMigrationsDbContext>()
            .UseNpgsql(configuration.GetConnectionString("Default"));

        return new InventoryTrackingAutomationHttpApiHostMigrationsDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}