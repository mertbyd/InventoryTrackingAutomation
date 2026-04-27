using InventoryTrackingAutomation.Entities.Workflows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Configurations.Workflows;

/// <summary>
/// WorkflowDefinition entity'si için veritabanı şema konfigürasyonu.
/// </summary>
public class WorkflowDefinitionConfiguration : IEntityTypeConfiguration<WorkflowDefinition>
{
    public void Configure(EntityTypeBuilder<WorkflowDefinition> builder)
    {
        builder.ToTable(InventoryTrackingAutomationDbProperties.DbTablePrefix + "WorkflowDefinitions", InventoryTrackingAutomationDbProperties.WorkflowSchema);
        
        builder.ConfigureByConvention(); // ABP'nin standart audit kolonlarını ekler

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(500);

        // Steps ilişkisi (Cascade Delete)
        builder.HasMany(x => x.Steps)
            .WithOne(x => x.WorkflowDefinition)
            .HasForeignKey(x => x.WorkflowDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
