using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Shipments;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Configurations.Shipments;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("Shipments", InventoryTrackingAutomationDbProperties.ShipmentSchema);
        builder.ConfigureByConvention();

        builder.Property(x => x.ShipmentNumber).IsRequired().HasMaxLength(50);

        builder.HasIndex(x => x.ShipmentNumber).IsUnique();

        builder.HasOne<Vehicle>()
            .WithMany()
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Worker>()
            .WithMany()
            .HasForeignKey(x => x.DriverWorkerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
