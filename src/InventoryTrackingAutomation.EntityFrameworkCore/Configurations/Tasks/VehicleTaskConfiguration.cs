using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Tasks;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Configurations.Tasks;

public class VehicleTaskConfiguration : IEntityTypeConfiguration<VehicleTask>
{
    public void Configure(EntityTypeBuilder<VehicleTask> builder)
    {
        builder.ToTable("vehicle_tasks", InventoryTrackingAutomationDbProperties.OperationSchema);
        builder.ConfigureByConvention();

        builder.Property(x => x.VehicleId).IsRequired();
        builder.Property(x => x.InventoryTaskId).IsRequired();
        builder.Property(x => x.AssignedAt).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();

        builder.HasIndex(x => new { x.VehicleId, x.IsActive });

        builder.HasOne<Vehicle>()
            .WithMany()
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<InventoryTask>()
            .WithMany()
            .HasForeignKey(x => x.InventoryTaskId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
