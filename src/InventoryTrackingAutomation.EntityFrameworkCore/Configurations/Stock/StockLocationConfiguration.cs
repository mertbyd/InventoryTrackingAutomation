using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Stock;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Configurations.Stock;

public class StockLocationConfiguration : IEntityTypeConfiguration<StockLocation>
{
    public void Configure(EntityTypeBuilder<StockLocation> builder)
    {
        builder.ToTable("stock_locations", InventoryTrackingAutomationDbProperties.InventorySchema);
        builder.ConfigureByConvention();

        builder.Property(x => x.ProductId).IsRequired();
        builder.Property(x => x.LocationType).IsRequired();
        builder.Property(x => x.Quantity).IsRequired();
        builder.Property(x => x.ReservedQuantity).IsRequired();

        builder.HasIndex(x => new { x.ProductId, x.LocationType, x.WarehouseSiteId, x.VehicleId }).IsUnique();

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Site>()
            .WithMany()
            .HasForeignKey(x => x.WarehouseSiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Vehicle>()
            .WithMany()
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
