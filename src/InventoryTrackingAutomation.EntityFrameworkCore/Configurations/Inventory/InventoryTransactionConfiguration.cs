using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Entities.Inventory;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Configurations.Stock;

public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.ToTable("inventory_transactions", InventoryTrackingAutomationDbProperties.InventorySchema);
        builder.ConfigureByConvention();

        builder.Property(x => x.ProductId).IsRequired();
        builder.Property(x => x.TransactionType).IsRequired();
        builder.Property(x => x.Quantity).IsRequired();
        builder.Property(x => x.OccurredAt).IsRequired();
        builder.Property(x => x.Note).HasMaxLength(500);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RelatedMovementRequest)
            .WithMany()
            .HasForeignKey(x => x.RelatedMovementRequestId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
