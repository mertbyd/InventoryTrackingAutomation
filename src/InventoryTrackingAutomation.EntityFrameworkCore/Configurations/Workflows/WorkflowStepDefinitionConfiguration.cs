using InventoryTrackingAutomation.Entities.Workflows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Configurations.Workflows;

/// <summary>
/// WorkflowStepDefinition entity'si için veritabanı şema konfigürasyonu.
/// </summary>
public class WorkflowStepDefinitionConfiguration : IEntityTypeConfiguration<WorkflowStepDefinition>
{
    public void Configure(EntityTypeBuilder<WorkflowStepDefinition> builder)
    {
        builder.ToTable(InventoryTrackingAutomationDbProperties.DbTablePrefix + "WorkflowStepDefinitions", InventoryTrackingAutomationDbProperties.WorkflowSchema);
        
        builder.ConfigureByConvention(); // ABP'nin standart audit kolonlarını ekler

        builder.Property(x => x.RequiredRoleName).HasMaxLength(50);
        builder.Property(x => x.ResolverKey).HasMaxLength(100);
    }
}
