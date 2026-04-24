using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Movements;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Configurations.Movements;

public class MovementApprovalConfiguration : IEntityTypeConfiguration<MovementApproval>
{
    public void Configure(EntityTypeBuilder<MovementApproval> builder)
    {
        builder.ToTable("MovementApprovals", InventoryTrackingAutomationDbProperties.MovementSchema);
        builder.ConfigureByConvention();

        builder.Property(x => x.Note).HasMaxLength(1000);

        builder.HasOne<MovementRequest>()
            .WithMany()
            .HasForeignKey(x => x.MovementRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Worker>()
            .WithMany()
            .HasForeignKey(x => x.ApproverWorkerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
