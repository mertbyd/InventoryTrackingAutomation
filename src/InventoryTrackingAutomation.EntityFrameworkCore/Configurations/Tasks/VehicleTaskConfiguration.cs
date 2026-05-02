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
        builder.Property(x => x.TaskId).IsRequired();
        builder.Property(x => x.ResponsibleWorkerId).IsRequired();
        builder.Property(x => x.AssignedAt).IsRequired();

        builder.HasIndex(x => new { x.VehicleId, x.ReleasedAt });

        builder.HasOne<Vehicle>()
            .WithMany()
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<InventoryTask>()
            .WithMany()
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Worker>()
            .WithMany()
            .HasForeignKey(x => x.ResponsibleWorkerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
