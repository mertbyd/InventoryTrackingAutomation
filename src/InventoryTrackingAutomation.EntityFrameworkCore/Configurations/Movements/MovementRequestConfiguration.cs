using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Movements;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Configurations.Movements;

public class MovementRequestConfiguration : IEntityTypeConfiguration<MovementRequest>
{
    public void Configure(EntityTypeBuilder<MovementRequest> builder)
    {
        builder.ToTable("MovementRequests", InventoryTrackingAutomationDbProperties.MovementSchema);
        builder.ConfigureByConvention();

        builder.Property(x => x.RequestNumber).IsRequired().HasMaxLength(50);
        builder.Property(x => x.RequestNote).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.CancellationNote).HasMaxLength(1000);

        builder.HasIndex(x => x.RequestNumber).IsUnique();

        builder.HasOne<Worker>()
            .WithMany()
            .HasForeignKey(x => x.RequestedByWorkerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Site>()
            .WithMany()
            .HasForeignKey(x => x.SourceSiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Site>()
            .WithMany()
            .HasForeignKey(x => x.TargetSiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Vehicle>()
            .WithMany()
            .HasForeignKey(x => x.RequestedVehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
