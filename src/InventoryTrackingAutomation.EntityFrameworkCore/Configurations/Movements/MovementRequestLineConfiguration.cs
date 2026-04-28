using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Movements;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Configurations.Movements;

public class MovementRequestLineConfiguration : IEntityTypeConfiguration<MovementRequestLine>
{
    public void Configure(EntityTypeBuilder<MovementRequestLine> builder)
    {
        builder.ToTable("movement_request_lines", InventoryTrackingAutomationDbProperties.MovementSchema);
        builder.ConfigureByConvention();

        builder.HasOne<MovementRequest>()
            .WithMany()
            .HasForeignKey(x => x.MovementRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
