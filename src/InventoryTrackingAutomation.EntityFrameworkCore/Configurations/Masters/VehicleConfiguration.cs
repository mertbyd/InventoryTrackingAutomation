using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryTrackingAutomation.Entities.Masters;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Configurations.Masters;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles", InventoryTrackingAutomationDbProperties.MasterSchema);
        builder.ConfigureByConvention();

        builder.Property(x => x.PlateNumber).IsRequired().HasMaxLength(20);

        builder.HasIndex(x => x.PlateNumber).IsUnique();
    }
}
