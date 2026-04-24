using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace InventoryTrackingAutomation.EntityFrameworkCore;

public static class InventoryTrackingAutomationDbContextModelCreatingExtensions
{
    public static void ConfigureInventoryTrackingAutomation(
        this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        // IEntityTypeConfiguration<> dosyalarını (Configurations/ klasörü) otomatik tarar ve uygular.
        // Migration DbContext bu metodu çağırdığı için entity'ler migration'a dahil olur.
        builder.ApplyConfigurationsFromAssembly(
            typeof(InventoryTrackingAutomationDbContextModelCreatingExtensions).Assembly);
    }
}
