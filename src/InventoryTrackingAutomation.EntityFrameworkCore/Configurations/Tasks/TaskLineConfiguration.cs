using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Tasks;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Configurations.Tasks;

public class TaskLineConfiguration : IEntityTypeConfiguration<TaskLine>
{
    public void Configure(EntityTypeBuilder<TaskLine> builder)
    {
        builder.ToTable("task_lines", InventoryTrackingAutomationDbProperties.OperationSchema);
        builder.ConfigureByConvention();

        builder.Property(x => x.TaskId).IsRequired();
        builder.Property(x => x.ProductId).IsRequired();
        builder.Property(x => x.Quantity).IsRequired();

        builder.HasIndex(x => new { x.TaskId, x.ProductId }).IsUnique();

        builder.HasOne<InventoryTask>()
            .WithMany()
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
