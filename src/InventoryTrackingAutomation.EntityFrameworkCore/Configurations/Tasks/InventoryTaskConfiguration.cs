using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Tasks;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Configurations.Tasks;

public class InventoryTaskConfiguration : IEntityTypeConfiguration<InventoryTask>
{
    public void Configure(EntityTypeBuilder<InventoryTask> builder)
    {
        builder.ToTable("tasks", InventoryTrackingAutomationDbProperties.OperationSchema);
        builder.ConfigureByConvention();

        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Region).IsRequired().HasMaxLength(100);
        builder.Property(x => x.StartDate).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.IsActive).IsRequired();

        builder.HasIndex(x => x.Code).IsUnique();

        builder.HasOne<Warehouse>()
            .WithMany()
            .HasForeignKey(x => x.ReturnWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
