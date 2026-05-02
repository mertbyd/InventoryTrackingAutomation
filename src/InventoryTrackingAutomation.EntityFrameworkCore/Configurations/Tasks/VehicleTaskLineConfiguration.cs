using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryTrackingAutomation.Entities.Tasks;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Configurations.Tasks;

public class VehicleTaskLineConfiguration : IEntityTypeConfiguration<VehicleTaskLine>
{
    public void Configure(EntityTypeBuilder<VehicleTaskLine> builder)
    {
        builder.ToTable("vehicle_task_lines", InventoryTrackingAutomationDbProperties.OperationSchema);
        builder.ConfigureByConvention();

        builder.Property(x => x.VehicleTaskId).IsRequired();
        builder.Property(x => x.TaskLineId).IsRequired();
        builder.Property(x => x.AllocatedQuantity).IsRequired();
        builder.Property(x => x.ReceivedQuantity).IsRequired();
        builder.Property(x => x.DamagedQuantity).IsRequired();
        builder.Property(x => x.LostQuantity).IsRequired();
        builder.Property(x => x.ConsumedQuantity).IsRequired();
        builder.Property(x => x.ReceiveNote).HasMaxLength(500);

        builder.HasIndex(x => new { x.VehicleTaskId, x.TaskLineId }).IsUnique();

        builder.HasOne<VehicleTask>()
            .WithMany()
            .HasForeignKey(x => x.VehicleTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<TaskLine>()
            .WithMany()
            .HasForeignKey(x => x.TaskLineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
