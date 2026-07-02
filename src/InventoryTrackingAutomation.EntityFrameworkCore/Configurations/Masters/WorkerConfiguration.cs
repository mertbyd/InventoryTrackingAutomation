using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Entities.Masters;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Configurations.Masters;

public class WorkerConfiguration : IEntityTypeConfiguration<Worker>
{
    public void Configure(EntityTypeBuilder<Worker> builder)
    {
        builder.ToTable("workers", InventoryTrackingAutomationDbProperties.MasterSchema);
        builder.ConfigureByConvention();

        builder.Property(x => x.RegistrationNumber).HasMaxLength(20);

        builder.HasIndex(x => x.UserId).IsUnique();
        builder.HasIndex(x => x.RegistrationNumber).IsUnique();

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DefaultWarehouse)
            .WithMany()
            .HasForeignKey(x => x.DefaultWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Manager)
            .WithMany()
            .HasForeignKey(x => x.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.WorkerType)
            .WithMany()
            .HasForeignKey(x => x.WorkerTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
