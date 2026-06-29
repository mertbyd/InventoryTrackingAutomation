using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryTrackingAutomation.Entities.Lookups;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Configurations.Lookups;

public class WorkerTypeConfiguration : IEntityTypeConfiguration<WorkerType>
{
    public void Configure(EntityTypeBuilder<WorkerType> builder)
    {
        builder.ToTable("worker_types", InventoryTrackingAutomationDbProperties.LookupSchema);
        builder.ConfigureByConvention();

        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(500);

        builder.HasIndex(x => x.Code).IsUnique();
    }
}
