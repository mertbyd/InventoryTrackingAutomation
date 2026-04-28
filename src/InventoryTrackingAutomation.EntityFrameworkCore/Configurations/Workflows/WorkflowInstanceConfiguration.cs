using InventoryTrackingAutomation.Entities.Workflows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Configurations.Workflows;

/// <summary>
/// WorkflowInstance entity'si için veritabanı şema konfigürasyonu.
/// </summary>
public class WorkflowInstanceConfiguration : IEntityTypeConfiguration<WorkflowInstance>
{
    public void Configure(EntityTypeBuilder<WorkflowInstance> builder)
    {
        builder.ToTable("workflow_instances", InventoryTrackingAutomationDbProperties.WorkflowSchema);
        
        builder.ConfigureByConvention(); // ABP'nin standart audit kolonlarını ekler

        builder.Property(x => x.EntityType).IsRequired().HasMaxLength(50);

        // InstanceSteps ilişkisi (Cascade Delete)
        builder.HasMany(x => x.Steps)
            .WithOne(x => x.WorkflowInstance)
            .HasForeignKey(x => x.WorkflowInstanceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
